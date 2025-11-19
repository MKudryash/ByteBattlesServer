<template>
  <div class="task-template-builder-container10">
    <app-navigation></app-navigation>

    <div class="task-template-builder-container11">
      <div class="task-template-builder-container12">
        <DangerousHTML
            html="<style>
  .builder-container {
    min-height: 100vh;
    background: var(--color-surface);
    padding: var(--spacing-2xl) 0;
  }

  .builder-container::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-image:
      radial-gradient(circle at 80% 20%, color-mix(in srgb, var(--color-secondary) 6%, transparent) 0%, transparent 50%),
      repeating-linear-gradient(
        0deg,
        transparent,
        transparent 2px,
        color-mix(in srgb, var(--color-border) 5%, transparent) 2px,
        color-mix(in srgb, var(--color-border) 5%, transparent) 4px
      );
    pointer-events: none;
    z-index: 1;
  }

  .wizard-progress {
    position: sticky;
    top: 0;
    background: white;
    z-index: 100;
    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  }

  .retro-card {
    background: var(--color-surface-elevated);
    border: 1px solid var(--color-border);
    border-radius: var(--border-radius-lg);
    box-shadow: var(--shadow-level-1);
    position: relative;
  }

  .retro-card::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    height: 3px;
    background: linear-gradient(90deg, var(--color-primary), var(--color-secondary));
    border-radius: var(--border-radius-lg) var(--border-radius-lg) 0 0;
  }

  .vintage-border {
    border: 1px solid var(--color-border);
    border-radius: var(--border-radius-md);
    background: var(--color-surface);
    box-shadow:
      inset 0 1px 2px color-mix(in srgb, var(--color-on-surface) 5%, transparent),
      0 2px 4px color-mix(in srgb, var(--color-neutral) 8%, transparent);
  }

  @keyframes slideIn {
    from {
      opacity: 0;
      transform: translateY(20px);
    }
    to {
      opacity: 1;
      transform: translateY(0);
    }
  }

  .step-content {
    animation: slideIn 0.5s var(--animation-curve-primary);
  }

  @media (prefers-reduced-motion: reduce) {
    *, *::before, *::after {
      animation-duration: 0.01ms !important;
      animation-iteration-count: 1 !important;
      transition-duration: 0.01ms !important;
    }
  }
  </style>"
        ></DangerousHTML>
      </div>
    </div>

    <section class="builder-container" role="main" aria-label="Конструктор учебных задач">
      <div class="container">
        <!-- Заголовок и прогресс -->
        <div class="builder-header">
          <div class="title-section">
            <h1 class="builder-title">
              Конструктор учебных задач
            </h1>
            <p class="builder-subtitle">
              Создавайте структурированные задания в винтажном стиле для любого языка программирования
            </p>
          </div>

          <!-- Прогресс-бар -->
          <div class="wizard-progress retro-card">
            <div class="progress-bar">
              <div class="progress-fill" :style="{width: `${(currentStep / steps.length) * 100}%`}"></div>
            </div>
            <div class="steps-indicator">
              <span class="step-counter">Шаг {{ currentStep }} из {{ steps.length }}</span>
              <span class="step-name">{{ steps[currentStep - 1]?.name }}</span>
            </div>
          </div>
        </div>

        <div class="wizard-layout">
          <!-- Боковая панель с шагами -->
          <aside class="wizard-sidebar" role="navigation" aria-label="Шаги создания задачи">
            <nav class="steps-nav retro-card">
              <div
                  v-for="(step, index) in steps"
                  :key="index"
                  :class="['step-nav-item', {
                  'active': currentStep === index + 1,
                  'completed': currentStep > index + 1
                }]"
                  @click="goToStep(index + 1)"
              >
                <div class="step-nav-icon">
                  <span v-if="currentStep > index + 1">✓</span>
                  <span v-else>{{ index + 1 }}</span>
                </div>
                <div class="step-nav-content">
                  <h3>{{ step.name }}</h3>
                  <p>{{ step.description }}</p>
                </div>
              </div>
            </nav>

            <!-- Быстрый предпросмотр -->
            <div class="quick-preview retro-card">
              <h4>
                <span class="preview-icon">👁️</span>
                Быстрый предпросмотр
              </h4>
              <div class="preview-badges">
                <span class="preview-badge" v-if="taskData.title">
                  📌 {{ taskData.title }}
                </span>
                <span class="preview-badge" v-if="taskData.language">
                  💻 {{ getLanguageName(taskData.language) }}
                </span>
                <span class="preview-badge" v-if="taskData.functionName">
                  🔧 {{ taskData.functionName }}()
                </span>
                <span class="preview-badge" v-if="taskData.tests.length">
                  ✅ {{ taskData.tests.length }} тест{{ taskData.tests.length > 1 ? 'ов' : '' }}
                </span>
                <span class="preview-badge" v-if="taskData.difficulty">
                  🎯 {{ getDifficultyLabel(taskData.difficulty) }}
                </span>
              </div>
            </div>

            <!-- Статистика -->
            <div class="stats-preview retro-card">
              <h4>Статистика задачи</h4>
              <div class="stats-grid">
                <div class="stat-item">
                  <span class="stat-value">{{ taskData.parameters.length }}</span>
                  <span class="stat-label">параметров</span>
                </div>
                <div class="stat-item">
                  <span class="stat-value">{{ taskData.libraries.length }}</span>
                  <span class="stat-label">библиотек</span>
                </div>
                <div class="stat-item">
                  <span class="stat-value">{{ taskData.tests.length }}</span>
                  <span class="stat-label">тестов</span>
                </div>
                <div class="stat-item">
                  <span class="stat-value">{{ taskData.examples.length }}</span>
                  <span class="stat-label">примеров</span>
                </div>
              </div>
            </div>
          </aside>

          <!-- Основное содержимое -->
          <main class="wizard-main" role="region" :aria-label="`Шаг ${currentStep}: ${steps[currentStep - 1]?.name}`">
            <!-- Шаг 1: Основная информация -->
            <div v-if="currentStep === 1" class="step-content">
              <div class="step-header">
                <h2>
                  Основная информация о задаче
                </h2>
                <p>Дайте задаче понятное название и описание, чтобы студенты понимали, что от них требуется</p>
              </div>

              <div class="form-grid">
                <div class="form-section retro-card">
                  <h3>Основные данные</h3>

                  <div class="form-group">
                    <label for="task-title" class="required">
                      Название задачи
                    </label>
                    <div class="input-container vintage-border">
                      <input
                          type="text"
                          id="task-title"
                          v-model="taskData.title"
                          placeholder="Например: 'Сумма элементов массива'"
                          maxlength="100"
                      >
                    </div>
                    <div class="char-counter">{{ taskData.title.length }}/100</div>
                  </div>

                  <div class="form-group">
                    <label for="task-description" class="required">
                      Описание задачи
                    </label>
                    <div class="input-container vintage-border">
                      <textarea
                          id="task-description"
                          v-model="taskData.description"
                          rows="6"
                          placeholder="Подробно опишите условие задачи. Что должен сделать студент? Какие данные на входе? Что ожидается на выходе?"
                      ></textarea>
                    </div>
                    <div class="hint">
                      <span class="hint-icon">💡</span>
                      Используйте Markdown для форматирования текста
                    </div>
                  </div>
                </div>

                <div class="form-section retro-card">
                  <h3>Дополнительные настройки</h3>

                  <div class="form-group">
                    <label for="task-category">
                      Категория
                    </label>
                    <div class="input-container vintage-border">
                      <select id="task-category" v-model="taskData.category">
                        <option value="">Выберите категорию</option>
                        <option value="algorithms">Алгоритмы</option>
                        <option value="data-structures">Структуры данных</option>
                        <option value="oop">ООП</option>
                        <option value="web">Веб-разработка</option>
                        <option value="databases">Базы данных</option>
                        <option value="other">Другое</option>
                      </select>
                    </div>
                  </div>

                  <div class="form-group">
                    <label for="task-difficulty" class="required">
                      Уровень сложности
                    </label>
                    <div class="difficulty-selector">
                      <label
                          v-for="diff in difficultyLevels"
                          :key="diff.value"
                          :class="['difficulty-option vintage-border', { 'selected': taskData.difficulty === diff.value }]"
                      >
                        <input
                            type="radio"
                            v-model="taskData.difficulty"
                            :value="diff.value"
                            hidden
                        >
                        <span class="diff-icon">{{ diff.icon }}</span>
                        <span class="diff-label">{{ diff.label }}</span>
                      </label>
                    </div>
                  </div>

                  <div class="form-group">
                    <label for="task-tags">
                      Теги
                    </label>
                    <div class="tags-input vintage-border">
                      <div class="tags-list">
                        <span v-for="(tag, index) in taskData.tags" :key="index" class="tag">
                          {{ tag }}
                          <button @click="removeTag(index)" class="tag-remove">×</button>
                        </span>
                      </div>
                      <input
                          type="text"
                          v-model="newTag"
                          @keydown.enter="addTag"
                          placeholder="Введите тег и нажмите Enter"
                      >
                    </div>
                  </div>

                  <div class="form-group">
                    <label for="time-estimate">
                      Примерное время выполнения
                    </label>
                    <div class="time-estimate vintage-border">
                      <input
                          type="number"
                          id="time-estimate"
                          v-model.number="taskData.timeEstimate"
                          min="5"
                          max="180"
                      >
                      <span>минут</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Пример оформления -->
              <div class="example-section retro-card">
                <h3>Пример хорошего описания</h3>
                <div class="example-content">
                  <div class="example-bad">
                    <h4>Плохо:</h4>
                    <p>"Напишите функцию, которая что-то делает с массивом"</p>
                  </div>
                  <div class="example-good">
                    <h4>Хорошо:</h4>
                    <p>"Напишите функцию <code>findMax</code>, которая принимает массив целых чисел и возвращает максимальный элемент. Если массив пуст, функция должна вернуть <code>None</code> (Python) или <code>-1</code> (C++/Java)."</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Шаг 2: Сигнатура функции -->
            <div v-if="currentStep === 2" class="step-content">
              <div class="step-header">
                <h2>
                  Определение функции
                </h2>
                <p>Опишите функцию, которую должен реализовать студент. Укажите параметры, возвращаемое значение и контракты</p>
              </div>


                <div class="form-section retro-card">
                  <h3>Основная сигнатура</h3>

                  <div class="form-group">
                    <label for="function-name" class="required">
                      Имя функции
                    </label>
                    <div class="input-container vintage-border">
                      <input
                          type="text"
                          id="function-name"
                          v-model="taskData.functionName"
                          placeholder="calculateSum, findMax, processData..."
                      >
                    </div>
                  </div>

                  <div class="form-group">
                    <label>
                      Параметры функции
                    </label>
                    <div class="params-container vintage-border">
                      <div class="params-header">
                        <span>Имя параметра</span>
                        <span>Тип</span>
                        <span>По умолчанию</span>
                        <span>Описание</span>
                        <span></span>
                      </div>
                      <div
                          v-for="(param, index) in taskData.parameters"
                          :key="index"
                          class="param-row"
                      >
                        <input
                            type="text"
                            v-model="param.name"
                            placeholder="param1"
                            :class="['vintage-border', { 'error': !param.name && paramSubmitted }]"
                        >
                        <select v-model="param.type" class="vintage-border">
                          <option v-for="type in getAvailableTypes()" :key="type" :value="type">
                            {{ type }}
                          </option>
                        </select>
                        <input
                            type="text"
                            v-model="param.defaultValue"
                            placeholder="Необязательно"
                            class="vintage-border"
                        >
                        <input
                            type="text"
                            v-model="param.description"
                            placeholder="Описание параметра"
                            class="vintage-border"
                        >
                        <button
                            @click="removeParameter(index)"
                            class="btn-remove"
                            :disabled="taskData.parameters.length === 1"
                        >
                          ×
                        </button>
                      </div>
                      <button @click="addParameter" class="btn-outline btn-sm">
                        <span class="btn-icon">+</span>
                        Добавить параметр
                      </button>
                    </div>
                  </div>

                  <div class="form-group">
                    <label for="return-type">
                      Тип возвращаемого значения
                    </label>
                    <div class="input-container vintage-border">
                      <select id="return-type" v-model="taskData.returnType">
                        <option value="void">void (нет возврата)</option>
                        <option v-for="type in getAvailableTypes()" :key="type" :value="type">
                          {{ type }}
                        </option>
                      </select>
                    </div>
                  </div>
                </div>


              <!-- Предпросмотр сигнатуры -->
              <div class="preview-section retro-card">
                <h3>
                  Предпросмотр сигнатуры
                </h3>
                <div class="code-preview vintage-border">
                  <pre><code>{{ generateFunctionSignature() }}</code></pre>
                </div>
                <div class="hint">
                  Эта сигнатура будет автоматически подставлена в шаблон кода
                </div>
              </div>
            </div>

            <!-- Шаг 3: Конфигурация окружения -->
            <div v-if="currentStep === 3" class="step-content">
              <div class="step-header">
                <h2>
                  Конфигурация окружения
                </h2>
                <p>Выберите язык программирования, настройте окружение и необходимые зависимости</p>
              </div>

              <div class="form-grid">
                <div class="form-section retro-card">
                  <h3>Язык программирования</h3>

                  <div class="form-group">
                    <label for="language-select" class="required">
                      Основной язык
                    </label>
                    <div class="languages-grid">
                      <label
                          v-for="lang in availableLanguages"
                          :key="lang.id"
                          :class="['language-option vintage-border', { 'selected': taskData.language === lang.id }]"
                      >
                        <input
                            type="radio"
                            v-model="taskData.language"
                            :value="lang.id"
                            hidden
                        >
                        <div class="lang-icon">{{ lang.icon }}</div>
                        <div class="lang-info">
                          <strong>{{ lang.name }}</strong>
                          <span>{{ lang.version }}</span>-
                        </div>
                      </label>
                    </div>
                  </div>

                  <div class="form-group" v-if="taskData.language">
                    <label for="code-template">
                      Шаблон кода
                    </label>
                    <div class="input-container vintage-border">
                      <textarea
                          id="code-template"
                          v-model="taskData.codeTemplate"
                          rows="8"
                          placeholder="Базовый шаблон кода, который увидят студенты..."
                      ></textarea>
                    </div>
                    <div class="hint">
                      Используйте <code>{{function_signature}}</code> для автоматической вставки сигнатуры функции
                    </div>
                  </div>
                </div>

                <div class="form-section retro-card">
                  <h3>Библиотеки и зависимости</h3>

                  <div class="form-group">
                    <label>
                      Доступные библиотеки
                    </label>
                    <div class="libraries-panel vintage-border">
                      <div class="libraries-search">
                        <input
                            type="text"
                            v-model="librarySearch"
                            placeholder="Поиск библиотек..."
                            class="vintage-border"
                        >
                      </div>
                      <div class="libraries-list">
                        <div
                            v-for="lib in filteredLibraries"
                            :key="lib.id"
                            :class="['library-item vintage-border', { 'selected': isLibrarySelected(lib.id) }]"
                            @click="toggleLibrary(lib.id)"
                        >
                          <div class="lib-info">
                            <strong>{{ lib.name }}</strong>
                            <span>{{ lib.version }}</span>
                            <p class="lib-description">{{ lib.description }}</p>
                          </div>
                          <div class="lib-compatibility" :class="lib.compatibility">
                            {{ lib.compatibility === 'full' ? '✓ Совместима' : '⚠ Ограниченно' }}
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>

                  <div class="form-group" v-if="taskData.libraries.length > 0">
                    <label>
                      Выбранные библиотеки
                    </label>
                    <div class="selected-libraries">
                      <div
                          v-for="libId in taskData.libraries"
                          :key="libId"
                          class="selected-library vintage-border"
                      >
                        <span>{{ getLibraryName(libId) }}</span>
                        <button @click="toggleLibrary(libId)" class="btn-remove">×</button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Настройки выполнения -->
              <div class="form-section retro-card">
                <h3>Настройки выполнения</h3>
                <div class="execution-settings">
                  <div class="form-group">
                    <label for="time-limit">
                      Лимит времени (секунды)
                    </label>
                    <div class="input-container vintage-border">
                      <input
                          type="number"
                          id="time-limit"
                          v-model.number="taskData.timeLimit"
                          min="1"
                          max="30"
                      >
                    </div>
                  </div>

                  <div class="form-group">
                    <label for="memory-limit">
                      Лимит памяти (МБ)
                    </label>
                    <div class="input-container vintage-border">
                      <input
                          type="number"
                          id="memory-limit"
                          v-model.number="taskData.memoryLimit"
                          min="16"
                          max="1024"
                      >
                    </div>
                  </div>

                  <div class="form-group">
                    <label for="output-limit">
                      Лимит вывода (КБ)
                    </label>
                    <div class="input-container vintage-border">
                      <input
                          type="number"
                          id="output-limit"
                          v-model.number="taskData.outputLimit"
                          min="1"
                          max="1024"
                      >
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Шаг 4: Точка входа и примеры -->
            <div v-if="currentStep === 4" class="step-content">
              <div class="step-header">
                <h2>
                  Точка входа и примеры использования
                </h2>
                <p>Определите, как будет вызываться функция и какие примеры показывать студентам</p>
              </div>


                <div class="form-section retro-card">
                  <h3>Точка входа (main)</h3>

                  <div class="form-group">
                    <label for="main-template">
                      <span class="label-icon">📝</span>
                      Шаблон main функции
                    </label>
                    <div class="input-container vintage-border">
                      <textarea
                          id="main-template"
                          v-model="taskData.mainTemplate"
                          rows="10"
                          placeholder="Код, который будет выполняться при запуске программы..."
                      ></textarea>
                    </div>
                    <div class="hint">
                      Используйте <code>{{function_call}}}</code> для вызова студенческой функции
                    </div>
                  </div>

                  <div class="form-group">
                    <label for="input-format">
                      Формат входных данных
                    </label>
                    <div class="input-container vintage-border">
                      <textarea
                          id="input-format"
                          v-model="taskData.inputFormat"
                          rows="3"
                          placeholder="Опишите формат входных данных..."
                      ></textarea>
                    </div>
                  </div>

                  <div class="form-group">
                    <label for="output-format">
                      Формат выходных данных
                    </label>
                    <div class="input-container vintage-border">
                      <textarea
                          id="output-format"
                          v-model="taskData.outputFormat"
                          rows="3"
                          placeholder="Опишите формат выходных данных..."
                      ></textarea>
                    </div>
                  </div>
                </div>

            </div>

            <!-- Шаг 5: Тестирование -->
            <div v-if="currentStep === 5" class="step-content">
              <div class="step-header">
                <h2>
                  Система тестирования
                </h2>
                <p>Добавьте тесты для автоматической проверки решений студентов</p>
              </div>

              <div class="tests-management">
                <div class="tests-header">
                  <h3>Тестовые случаи</h3>
                </div>

                <div class="tests-list">
                  <div
                      v-for="(test, index) in taskData.tests"
                      :key="index"
                      :class="['test-case retro-card', { 'public': test.isPublic }]"
                  >
                    <div class="test-header">
                      <div class="test-info">
                        <h4>Тест {{ index + 1 }}</h4>
                        <div class="test-meta">
              <span class="test-visibility">
                {{ test.isPublic ? 'Публичный' : 'Скрытый' }}
              </span>
                          <span class="test-weight">Вес: {{ test.weight }}</span>
                        </div>
                      </div>
                      <div class="test-actions">
                        <button @click="toggleTestVisibility(index)" class="btn-sm btn-outline">
                          {{ test.isPublic ? 'Скрыть' : 'Показать' }}
                        </button>
                        <button @click="removeTest(index)" class="btn-remove">
                          Удалить
                        </button>
                      </div>
                    </div>

                    <div class="test-content">
                      <div class="test-io">
                        <div class="form-group">
                          <label>Входные данные</label>
                          <div class="input-container vintage-border">
                <textarea
                    v-model="test.input"
                    rows="3"
                    placeholder="Входные данные для теста"
                ></textarea>
                          </div>
                        </div>
                        <div class="form-group">
                          <label>Ожидаемый вывод</label>
                          <div class="input-container vintage-border">
                <textarea
                    v-model="test.expectedOutput"
                            rows="3"
                            placeholder="Ожидаемый результат"
                            ></textarea>
                          </div>
                        </div>
                      </div>


                    </div>
                  </div>

                  <button @click="addTest" class="btn-outline_left">
                    <span class="btn-icon">+</span>
                    Добавить тест
                  </button>
                </div>
              </div>
            </div>

            <!-- Навигация -->
            <div class="wizard-navigation">
              <div class="nav-left">
                <button
                    v-if="currentStep > 1"
                    @click="previousStep"
                    class="btn-outline_left"
                >
                  <span class="btn-icon">←</span>
                  Назад
                </button>
              </div>

              <div class="nav-center">
                <div class="step-info">
                  <span class="step-current">{{ currentStep }}</span>
                  <span class="step-separator">/</span>
                  <span class="step-total">{{ steps.length }}</span>
                </div>
              </div>

              <div class="nav-right">
                <button
                    v-if="currentStep < steps.length"
                    @click="validateAndNext"
                    class="btn-primary"
                    :disabled="!canProceed"
                >
                  Продолжить
                  <span class="btn-icon">→</span>
                </button>
                <button
                    v-else
                    @click="saveTask"
                    class="btn-accent"
                    :disabled="!canSave || isSaving"
                >
                  <span class="btn-icon">💾</span>
                  {{ isSaving ? 'Сохранение...' : 'Сохранить задачу' }}
                </button>

                <button @click="saveDraft" class="btn-text">
                  <span class="btn-icon">📄</span>
                  Черновик
                </button>
              </div>
            </div>
          </main>
        </div>
      </div>
    </section>

    <app-footer></app-footer>
  </div>
</template>

<script>
import DangerousHTML from 'dangerous-html/vue'
import AppNavigation from '../components/navigation'
import AppFooter from '../components/footer'
import { languageAPI, taskAPI } from '../api/task.js'
export default {
  name: 'TaskTemplateBuilder',
  components: {
    AppNavigation,
    DangerousHTML,
    AppFooter,
  },
  data() {
    return {
      currentStep: 1,
      paramSubmitted: false,
      librarySearch: '',
      isLoadingLibraries: false,
      newTag: '',
      isSaving: false,
      isLoading: false, // Добавьте это
      error: null, // Добавьте это
      steps: [
        { name: 'Основная информация', description: 'Название и описание задачи' },
        { name: 'Сигнатура функции', description: 'Определение функции и параметров' },
        { name: 'Конфигурация', description: 'Язык, библиотеки и окружение' },
        { name: 'Примеры использования', description: 'Точка входа и примеры' },
        { name: 'Тестирование', description: 'Тесты и проверка решений' }
      ],
      taskData: {
        title: '',
        description: '',
        category: '',
        difficulty: 'medium',
        tags: [],
        timeEstimate: 30,

        functionName: '',
        parameters: [{ name: '', type: 'int', defaultValue: '', description: '' }],
        returnType: 'void',
        functionDescription: '',
        preConditions: [],
        postConditions: [],
        timeComplexity: '',

        language: '',
        codeTemplate: '',
        libraries: [],
        timeLimit: 10,
        memoryLimit: 256,
        outputLimit: 64,

        mainTemplate: '',
        inputFormat: '',
        outputFormat: '',
        examples: [{
          description: '',
          input: '',
          output: '',
          isPublic: true
        }],

        tests: [{
          input: '',
          expectedOutput: '', // Изменили с output на expectedOutput
          isPublic: true,
          weight: 5,
          checkType: 'exact',
          customCheck: ''
        }],

        autoGrade: true,
        showDetailedErrors: false,
        allowCustomTests: false
      },
      difficultyLevels: [
        { value: 'easy', label: 'Начинающий', icon: '🌱' },
        { value: 'medium', label: 'Средний', icon: '🎯' },
        { value: 'hard', label: 'Продвинутый', icon: '🚀' },
        { value: 'expert', label: 'Эксперт', icon: '🏆' }
      ],
      availableLanguages: [],
      availableLibraries: [
        { id: 'numpy', name: 'NumPy', version: '1.23.0', description: 'Библиотека для научных вычислений', compatibility: 'full' },
        { id: 'pandas', name: 'Pandas', version: '1.5.3', description: 'Инструменты для анализа данных', compatibility: 'full' },
        { id: 'matplotlib', name: 'Matplotlib', version: '3.7.1', description: 'Библиотека для визуализации', compatibility: 'full' },
        { id: 'requests', name: 'Requests', version: '2.28.2', description: 'HTTP библиотека для Python', compatibility: 'full' },
        { id: 'junit', name: 'JUnit', version: '5.9.0', description: 'Фреймворк для тестирования', compatibility: 'full' },
        { id: 'mockito', name: 'Mockito', version: '4.11.0', description: 'Библиотека для мокирования', compatibility: 'limited' },
        { id: 'boost', name: 'Boost', version: '1.80.0', description: 'Набор библиотек для C++', compatibility: 'full' },
        { id: 'catch2', name: 'Catch2', version: '3.3.0', description: 'Фреймворк для тестирования C++', compatibility: 'full' }
      ]
    }
  },
  async mounted() {
  await this.loadLanguages()
  },
  computed: {
    canProceed() {
      switch (this.currentStep) {
        case 1:
          return this.taskData.title.trim() && this.taskData.description.trim()
        case 2:
          return this.taskData.functionName.trim() &&
              this.taskData.parameters.every(p => p.name.trim())
        case 3:
          return this.taskData.language
        case 4:
          return this.taskData.mainTemplate.trim()
        default:
          return true
      }
    },
    canSave() {
      // Безопасная проверка тестов
      const hasValidTests = this.taskData.tests.some(test =>
          test.input.trim() && test.expectedOutput.trim() // Исправлено имя поля
      );

      return this.canProceed && hasValidTests;
    },
    filteredLibraries() {
      if (!this.librarySearch) return this.availableLibraries
      return this.availableLibraries.filter(lib =>
          lib.name.toLowerCase().includes(this.librarySearch.toLowerCase()) ||
          lib.description.toLowerCase().includes(this.librarySearch.toLowerCase())
      )
    }
  },
  methods: {
    async loadLanguages() {
      this.isLoading = true
      this.error = null

      try {
        const languages = await languageAPI.getAll()

        // Преобразуем полученные данные в нужный формат
        this.availableLanguages = languages.map(lang => ({
          id: lang.id,
          name: lang.title || 'Unknown Language',
          shortName: lang.shortTitle || lang.title?.substring(0, 3).toUpperCase() || 'UNK',
          version: lang.version || '1.0',
          fileExtension: lang.fileExtension || '.txt',
          compilerCommand: lang.compilerCommand,
          executionCommand: lang.executionCommand,
          supportsCompilation: lang.supportsCompilation || false,
          patternMain: lang.patternMain,
          patternFunction: lang.patternFunction,
          icon: this.getLanguageIcon(lang.title || lang.shortTitle || lang.id),
          // Сохраняем библиотеки из ответа API
          libraries: lang.libraries ? lang.libraries.map(lib => ({
            id: lib.id,
            name: lib.name || 'Unknown Library',
            version: lib.version || '1.0.0',
            description: lib.description || 'No description available',
            languageId: lib.languageId,
            compatibility: 'full'
          })) : []
        }))

        console.log(`Загружено ${this.availableLanguages.length} языков программирования`)
        // Логируем библиотеки для отладки
        this.availableLanguages.forEach(lang => {
          console.log(`Язык ${lang.name}: ${lang.libraries.length} библиотек`, lang.libraries)
        })

      } catch (error) {
        console.error('Ошибка при загрузке языков:', error)
        this.error = 'Не удалось загрузить список языков'

        if (error.message.includes('401') || error.message.includes('403')) {
          this.error = 'Ошибка авторизации. Проверьте токен доступа.'
        } else if (error.message.includes('Network Error')) {
          this.error = 'Проблемы с подключением к серверу'
        }
      } finally {
        this.isLoading = false
      }
    },
    async loadLibrariesForLanguage(languageId) {
      if (!languageId) {
        this.availableLibraries = [];
        return;
      }

      this.isLoadingLibraries = true;
      try {
        console.log('Loading libraries for language:', languageId);


        const selectedLanguage = this.availableLanguages.find(lang => lang.id === languageId);

        if (selectedLanguage && selectedLanguage.libraries && selectedLanguage.libraries.length > 0) {
          // Используем библиотеки из уже загруженных данных
          this.availableLibraries = selectedLanguage.libraries;
          console.log(`Loaded ${this.availableLibraries.length} libraries from cached data`);
        } else {
          console.log('Using default libraries for language:', languageId);
        }

      } catch (error) {
        console.error('Error loading libraries:', error);
      } finally {
        this.isLoadingLibraries = false;
      }
    },
    getLanguageIcon(languageName) {
      const iconMap = {
        'python': '🐍',
        'java': '☕',
        'javascript': '📜',
        'typescript': '🔷',
        'cpp': '⚡',
        'csharp': '🎵',
        'go': '🐹',
        'rust': '🦀',
        'ruby': '💎',
        'php': '🐘',
        'swift': '🐦',
        'kotlin': '🔶'
      }

      const lowerName = languageName.toLowerCase()
      return iconMap[lowerName] || '💻'
    },
    goToStep(step) {
      if (step <= this.currentStep) {
        this.currentStep = step
      }
    },

    nextStep() {
      if (this.currentStep < this.steps.length) {
        this.currentStep++
      }
    },

    previousStep() {
      if (this.currentStep > 1) {
        this.currentStep--
      }
    },

    validateAndNext() {
      this.paramSubmitted = true
      if (this.canProceed) {
        this.nextStep()
      }
    },

    addParameter() {
      this.taskData.parameters.push({
        name: '',
        type: 'int',
        defaultValue: '',
        description: ''
      })
    },

    removeParameter(index) {
      if (this.taskData.parameters.length > 1) {
        this.taskData.parameters.splice(index, 1)
      }
    },

    addTag() {
      if (this.newTag.trim() && !this.taskData.tags.includes(this.newTag.trim())) {
        this.taskData.tags.push(this.newTag.trim())
        this.newTag = ''
      }
    },

    removeTag(index) {
      this.taskData.tags.splice(index, 1)
    },

    toggleLibrary(libId) {
      const index = this.taskData.libraries.indexOf(libId)
      if (index > -1) {
        this.taskData.libraries.splice(index, 1)
      } else {
        this.taskData.libraries.push(libId)
      }
    },

    isLibrarySelected(libId) {
      return this.taskData.libraries.includes(libId)
    },

    getLibraryName(libId) {
      const lib = this.availableLibraries.find(l => l.id === libId)
      return lib ? lib.name : libId
    },


    addTest() {
      this.taskData.tests.push({
        input: '',
        expectedOutput: '',
        isPublic: false,
        weight: 5,
        checkType: 'exact',
        customCheck: ''
      })
    },

    removeTest(index) {
      if (this.taskData.tests.length > 1) {
        this.taskData.tests.splice(index, 1)
      }
    },

    toggleTestVisibility(index) {
      this.taskData.tests[index].isPublic = !this.taskData.tests[index].isPublic
    },


    getAvailableTypes() {
      const baseTypes = ['int', 'float', 'double', 'string', 'boolean', 'char', 'byte']
      const collectionTypes = ['array', 'list', 'vector', 'map', 'dictionary', 'set']

      if (this.taskData.language === 'python') {
        return [...baseTypes, 'list', 'dict', 'tuple', 'set', 'None']
      } else if (this.taskData.language === 'java') {
        return [...baseTypes, 'List', 'ArrayList', 'Map', 'HashMap', 'Set', 'HashSet']
      } else if (this.taskData.language === 'cpp') {
        return [...baseTypes, 'vector', 'array', 'map', 'set', 'string']
      }

      return [...baseTypes, ...collectionTypes]
    },

    generateFunctionSignature() {
      if (!this.taskData.functionName) return '// Введите имя функции'

      const params = this.taskData.parameters
          .filter(p => p.name)
          .map(p => {
            let paramStr = p.name
            if (p.type) paramStr += `: ${p.type}`
            if (p.defaultValue) paramStr += ` = ${p.defaultValue}`
            return paramStr
          })
          .join(', ')

      let signature = `${this.taskData.functionName}(${params})`

      if (this.taskData.returnType !== 'void') {
        signature += ` -> ${this.taskData.returnType}`
      }

      return signature
    },

    getLanguageName(langId) {
      const lang = this.availableLanguages.find(l => l.id === langId)
      return lang ? lang.name : langId
    },

    getDifficultyLabel(difficulty) {
      const diff = this.difficultyLevels.find(d => d.value === difficulty)
      return diff ? diff.label : difficulty
    },

    saveDraft() {
      console.log('Сохранение черновика:', this.taskData)
      alert('Черновик сохранен!')
    },
    saveTask() {
      // Проверяем, есть ли валидные тесты
      const hasValidTests = this.taskData.tests.some(test =>
          test.input.trim() && test.expectedOutput.trim()
      );

      if (!hasValidTests) {
        alert('Добавьте хотя бы один тест с входными данными и ожидаемым выводом');
        return;
      }

      // Получаем реального пользователя (замените на ваш способ получения)
      const currentUser = this.getCurrentUser();

      // Подготавливаем данные для отправки согласно схеме API
      const taskToSave = {
        title: this.taskData.title,
        description: this.taskData.description,
        difficulty: this.taskData.difficulty,
        author: currentUser,
        functionName: this.taskData.functionName,
        patternMain: this.taskData.mainTemplate,
        patternFunction: this.taskData.codeTemplate,
        languageId: this.formatLanguageIds()
      };

      console.log('Подготовленные данные для сохранения:', taskToSave);

      // Отправляем на сервер
      this.sendTaskToServer(taskToSave);
    },

// Метод для получения текущего пользователя
    getCurrentUser() {
      // Замените на ваш способ получения текущего пользователя
      // Например, из Vuex store, localStorage, или другого места
      return localStorage.getItem('currentUser') || 'default_user';
    },

// Вспомогательные методы для форматирования данных
    formatInputParameters() {
      // Форматируем параметры в строку
      return this.taskData.parameters
          .filter(param => param.name.trim()) // Только заполненные параметры
          .map(param => {
            let paramStr = `${param.name}: ${param.type}`;
            if (param.defaultValue) {
              paramStr += ` = ${param.defaultValue}`;
            }
            if (param.description) {
              paramStr += ` // ${param.description}`;
            }
            return paramStr;
          })
          .join(', ');
    },

    formatLanguageIds() {
      // Преобразуем выбранный язык в массив UUID
      // Если поддерживается только один язык, возвращаем массив с одним элементом
      return this.taskData.language ? [this.taskData.language] : [];
    },

// Метод для отправки на сервер
    // Метод для отправки на сервер
    async sendTaskToServer(taskData) {
      try {
        this.isSaving = true;

        console.log('Отправка задачи на сервер...', taskData);
        const response = await taskAPI.create(taskData);

        console.log('Задача успешно создана:', response);

        // Если задача создана успешно, сохраняем тестовые случаи
        if (response && response.id) {
          console.log('ID созданной задачи:', response.id);
          await this.saveTestCases(response.id);
        } else {
          console.error('Не получен ID созданной задачи');
          alert('Задача создана, но не удалось получить ID для сохранения тестов');
          this.isSaving = false;
        }

      } catch (error) {
        console.error('Ошибка при сохранении задачи:', error);

        // Более информативное сообщение об ошибке
        let errorMessage = 'Ошибка при сохранении задачи';
        if (error.response && error.response.data) {
          const errorData = error.response.data;
          errorMessage += `: ${errorData.title || errorData.detail || JSON.stringify(errorData)}`;
        } else {
          errorMessage += `: ${error.message}`;
        }

        alert(errorMessage);
        this.isSaving = false;
      }
    },

    async saveTestCases(taskId) {
      try {
        const testCasesDto = {
          testCases: this.taskData.tests
              .filter(test => test.input.trim() && test.expectedOutput.trim())
              .map(test => ({
                input: test.input.trim(),
                output: test.expectedOutput.trim(),
                isPublic: test.isPublic || false
              }))
        };

        console.log('Сохранение тестовых случаев:', testCasesDto);

        if (testCasesDto.testCases.length > 0) {
          // Отправляем DTO объект, а не массив напрямую
          await taskAPI.createTestCases(taskId, testCasesDto);
          console.log('Тестовые случаи успешно сохранены');
          alert('Задача и тестовые случаи успешно созданы!');

          // Перенаправляем на страницу задач
          this.$router.push('/tasks');
        } else {
          alert('Задача успешно создана! (без тестовых случаев)');
          this.$router.push('/tasks');
        }

      } catch (error) {
        console.error('Ошибка при сохранении тестовых случаев:', error);

        let errorMessage = 'Задача создана, но не удалось сохранить тестовые случаи';

        if (error.response && error.response.data) {
          const errorData = error.response.data;
          errorMessage += `: ${errorData.title || errorData.detail || 'Неизвестная ошибка сервера'}`;
          console.error('Полная ошибка сервера:', errorData);
        } else {
          errorMessage += `: ${error.message}`;
        }

        alert(errorMessage);
        this.$router.push('/tasks');
      } finally {
        this.isSaving = false;
      }
    },
    generateLanguageSpecificSignature() {
      if (!this.taskData.functionName || !this.taskData.language) return '';

      const language = this.availableLanguages.find(lang => lang.id === this.taskData.language);
      if (!language) return '';

      const params = this.taskData.parameters
          .filter(p => p.name.trim())
          .map(p => {
            const paramName = p.name.trim();
            let paramStr = paramName;

            if (p.type && this.supportsTypeAnnotations(this.taskData.language)) {
              paramStr = this.formatParameterWithType(paramName, p.type, this.taskData.language);
            }

            if (p.defaultValue && p.defaultValue.trim()) {
              paramStr += this.formatDefaultValue(p.defaultValue, this.taskData.language);
            }

            return paramStr;
          })
          .join(', ');

      const functionName = this.taskData.functionName.trim();
      const returnType = this.taskData.returnType;

      return this.formatFunctionSignature(functionName, params, returnType, this.taskData.language);
    },

    // Проверка поддержки аннотаций типов для языка
    supportsTypeAnnotations(languageId) {
      const typedLanguages = ['java', 'cpp', 'csharp', 'typescript'];
      const language = this.availableLanguages.find(lang => lang.id === languageId);
      return typedLanguages.includes(language?.shortName?.toLowerCase()) || false;
    },

    // Форматирование параметра с типом
    formatParameterWithType(paramName, paramType, languageId) {
      const language = this.availableLanguages.find(lang => lang.id === languageId);
      const langShortName = language?.shortName?.toLowerCase();

      switch (langShortName) {
        case 'python':
          return `${paramName}: ${this.mapTypeToLanguage(paramType, languageId)}`;
        case 'java':
        case 'cpp':
        case 'csharp':
          return `${this.mapTypeToLanguage(paramType, languageId)} ${paramName}`;
        default:
          return paramName;
      }
    },

    // Форматирование значения по умолчанию
    formatDefaultValue(defaultValue, languageId) {
      const language = this.availableLanguages.find(lang => lang.id === languageId);
      const langShortName = language?.shortName?.toLowerCase();

      switch (langShortName) {
        case 'python':
          return ` = ${defaultValue}`;
        case 'cpp':
        case 'csharp':
        case 'java':
          return ` = ${defaultValue}`;
        default:
          return ` = ${defaultValue}`;
      }
    },

    // Форматирование полной сигнатуры функции
    formatFunctionSignature(functionName, params, returnType, languageId) {
      const language = this.availableLanguages.find(lang => lang.id === languageId);
      const langShortName = language?.shortName?.toLowerCase();

      let signature = `${functionName}(${params})`;

      // Добавляем возвращаемый тип
      if (returnType && returnType !== 'void') {
        switch (langShortName) {
          case 'python':
            signature += ` -> ${this.mapTypeToLanguage(returnType, languageId)}`;
            break;
          case 'java':
          case 'cpp':
          case 'csharp':
            signature = `${this.mapTypeToLanguage(returnType, languageId)} ${signature}`;
            break;
        }
      } else if (returnType === 'void' && langShortName !== 'python') {
        signature = `void ${signature}`;
      }

      return signature;
    },

    // Маппинг типов на конкретный язык
    mapTypeToLanguage(type, languageId) {
      const language = this.availableLanguages.find(lang => lang.id === languageId);
      const langShortName = language?.shortName?.toLowerCase();

      const typeMap = {
        python: {
          'int': 'int',
          'float': 'float',
          'double': 'float',
          'string': 'str',
          'boolean': 'bool',
          'char': 'str',
          'byte': 'bytes',
          'array': 'list',
          'list': 'list',
          'vector': 'list',
          'map': 'dict',
          'dictionary': 'dict',
          'set': 'set',
          'void': 'None'
        },
        java: {
          'int': 'int',
          'float': 'float',
          'double': 'double',
          'string': 'String',
          'boolean': 'boolean',
          'char': 'char',
          'byte': 'byte',
          'array': 'array',
          'list': 'List',
          'vector': 'Vector',
          'map': 'Map',
          'dictionary': 'Dictionary',
          'set': 'Set',
          'void': 'void'
        },
        cpp: {
          'int': 'int',
          'float': 'float',
          'double': 'double',
          'string': 'std::string',
          'boolean': 'bool',
          'char': 'char',
          'byte': 'unsigned char',
          'array': 'std::array',
          'list': 'std::list',
          'vector': 'std::vector',
          'map': 'std::map',
          'dictionary': 'std::map',
          'set': 'std::set',
          'void': 'void'
        },
        csharp: {
          'int': 'int',
          'float': 'float',
          'double': 'double',
          'string': 'string',
          'boolean': 'bool',
          'char': 'char',
          'byte': 'byte',
          'array': 'array',
          'list': 'List',
          'vector': 'List',
          'map': 'Dictionary',
          'dictionary': 'Dictionary',
          'set': 'HashSet',
          'void': 'void'
        }
      };

      const langMap = typeMap[langShortName] || typeMap.python;
      return langMap[type] || type;
    },

    // Генерация шаблона main функции для выбранного языка
    generateMainTemplate() {
      if (!this.taskData.language) return '';

      const language = this.availableLanguages.find(lang => lang.id === this.taskData.language);
      if (!language || !language.patternMain) return '';

      // Заменяем плейсхолдеры в шаблоне main
      let mainTemplate = language.patternMain;

      // Добавляем вызов функции, если есть сигнатура
      if (this.taskData.functionName) {
        const functionCall = this.generateFunctionCall();
        mainTemplate = mainTemplate.replace('/* код */', functionCall + '\n    /* ваш код для тестирования */');
      }

      return mainTemplate;
    },

    // Генерация вызова функции
    generateFunctionCall() {
      if (!this.taskData.functionName) return '';

      const params = this.taskData.parameters
          .filter(p => p.name.trim())
          .map(p => {
            // Генерируем примеры значений для параметров
            return this.generateExampleValue(p.type, p.name);
          })
          .join(', ');

      const functionName = this.taskData.functionName.trim();

      // Для языков, где нужно сохранить результат
      if (this.taskData.returnType && this.taskData.returnType !== 'void') {
        const language = this.availableLanguages.find(lang => lang.id === this.taskData.language);
        const langShortName = language?.shortName?.toLowerCase();

        let resultType = this.mapTypeToLanguage(this.taskData.returnType, this.taskData.language);
        let resultVar = 'result';

        switch (langShortName) {
          case 'python':
            return `${resultVar} = ${functionName}(${params})`;
          case 'java':
          case 'cpp':
          case 'csharp':
            return `${resultType} ${resultVar} = ${functionName}(${params});`;
          default:
            return `${functionName}(${params});`;
        }
      } else {
        return `${functionName}(${params});`;
      }
    },

    // Генерация примеров значений для параметров
    generateExampleValue(type, paramName) {
      const examples = {
        'int': '42',
        'float': '3.14',
        'double': '3.14',
        'string': `"${paramName}"`,
        'boolean': 'true',
        'char': `'a'`,
        'array': `[1, 2, 3]`,
        'list': `[1, 2, 3]`,
        'vector': `{1, 2, 3}`,
        'map': `{"key": "value"}`,
        'dictionary': `{"key": "value"}`
      };

      return examples[type] || 'null';
    },

    // Обработчик смены языка
    onLanguageChange() {
      // Автоматически обновляем шаблон кода с правильной сигнатурой
      this.updateCodeTemplate();

      // Автоматически обновляем шаблон main
      this.updateMainTemplate();
    },

    // Обновление шаблона кода
    updateCodeTemplate() {
      if (!this.taskData.language) return;

      const language = this.availableLanguages.find(lang => lang.id === this.taskData.language);
      if (!language) return;

      const functionSignature = this.generateLanguageSpecificSignature();

      if (language.patternFunction) {
        // Используем шаблон из базы данных
        this.taskData.codeTemplate = language.patternFunction.replace('{{function_signature}}', functionSignature);
      } else {
        // Генерируем базовый шаблон
        this.taskData.codeTemplate = `${functionSignature} {\n    // ваш код здесь\n}`;
      }
    },

    // Обновление шаблона main
    updateMainTemplate() {
      this.taskData.mainTemplate = this.generateMainTemplate();
    },
  },
  watch: {
    'taskData.language': {
      handler(newLanguageId) {
        if (newLanguageId) {
          console.log('Language changed to:', newLanguageId);
          this.loadLibrariesForLanguage(newLanguageId);
          // Очищаем выбранные библиотеки при смене языка
          this.taskData.libraries = [];

          // Автоматически обновляем шаблоны
          this.$nextTick(() => {
            this.updateCodeTemplate();
            this.updateMainTemplate();
          });
        } else {
          this.availableLibraries = [];
        }
      },
      immediate: true
    },

    // Следим за изменениями в сигнатуре функции и обновляем шаблоны
    'taskData.functionName': function() {
      if (this.taskData.language) {
        this.$nextTick(() => {
          this.updateCodeTemplate();
          this.updateMainTemplate();
        });
      }
    },

    'taskData.parameters': {
      handler() {
        if (this.taskData.language) {
          this.$nextTick(() => {
            this.updateCodeTemplate();
            this.updateMainTemplate();
          });
        }
      },
      deep: true
    },

    'taskData.returnType': function() {
      if (this.taskData.language) {
        this.$nextTick(() => {
          this.updateCodeTemplate();
          this.updateMainTemplate();
        });
      }
    }

  },
  metaInfo: {
    title: 'Конструктор задач',
    meta: [
      {
        property: 'og:title',
        content: 'Конструктор задач',
      },
    ],
  },
}
</script>

<style scoped>

.task-template-builder-container10 {
  width: 100%;
  display: block;
  min-height: 100vh;
  font-family: var(--font-family-body);
  background: var(--color-surface);
}

.task-template-builder-container11 {
  display: none;
}

.task-template-builder-container12 {
  display: contents;
}

.container {
  max-width: var(--content-max-width);
  margin: 0 auto;
  padding: 0 var(--spacing-lg);

}

/* Заголовок */
.builder-header {
  margin-bottom: var(--spacing-2xl);
}

.title-section {
  text-align: center;
  margin-bottom: var(--spacing-xl);
}

.builder-title {
  color: var(--color-on-surface);
  font-size: var(--font-size-hero);
  margin-bottom: var(--spacing-md);
  font-family: var(--font-family-heading);
  font-weight: var(--font-weight-heading);
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--spacing-md);
  line-height: var(--line-height-heading);
}

.title-icon {
  font-size: var(--font-size-xl);
}

.builder-subtitle {
  color: var(--color-on-surface-secondary);
  font-size: var(--font-size-lg);
  margin-bottom: var(--spacing-xl);
  line-height: var(--line-height-body);
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
}

/* Прогресс-бар */
.wizard-progress {
  padding: var(--spacing-lg);
}

.progress-bar {
  height: 8px;
  background: var(--color-backplate);
  border-radius: var(--border-radius-full);
  overflow: hidden;
  margin-bottom: var(--spacing-md);
  border: 1px solid var(--color-border);
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, var(--color-primary), var(--color-secondary));
  transition: width var(--animation-duration-slow) var(--animation-curve-primary);
  border-radius: var(--border-radius-full);
}

.steps-indicator {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: var(--font-size-sm);
}

.step-counter {
  color: var(--color-on-surface-secondary);
  font-weight: var(--font-weight-body);
}

.step-name {
  color: var(--color-on-surface);
  font-weight: var(--font-weight-heading);
  background: var(--color-primary);
  padding: var(--spacing-xs) var(--spacing-md);
  border-radius: var(--border-radius-full);
}

/* Основной лейаут */
.wizard-layout {
  display: grid;
  grid-template-columns: 320px 1fr;
  gap: var(--spacing-xl);
  align-items: start;
  margin-bottom: var(--spacing-2xl);
}

.wizard-sidebar {
  position: sticky;
  top: var(--spacing-xl);
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

/* Навигация по шагам */
.steps-nav {
  padding: var(--spacing-lg);
}

.step-nav-item {
  display: flex;
  align-items: flex-start;
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  border-radius: var(--border-radius-md);
  cursor: pointer;
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
  margin-bottom: var(--spacing-sm);
  border: 1px solid transparent;
}

.step-nav-item:last-child {
  margin-bottom: 0;
}

.step-nav-item:hover {
  background: var(--color-backplate);
  border-color: var(--color-border);
  transform: translateX(var(--spacing-xs));
}

.step-nav-item.active {
  background: color-mix(in srgb, var(--color-primary) 12%, transparent);
  border-color: var(--color-primary);
  border-left: 4px solid var(--color-primary);
}

.step-nav-item.completed {
  background: color-mix(in srgb, var(--color-accent) 8%, transparent);
  border-color: var(--color-accent);
}

.step-nav-icon {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: var(--color-border);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: var(--font-weight-heading);
  flex-shrink: 0;
  font-size: var(--font-size-sm);
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
  border: 2px solid transparent;
}

.step-nav-item.active .step-nav-icon {
  background: var(--color-primary);
  color: var(--color-on-primary);
  border-color: var(--color-primary);
}

.step-nav-item.completed .step-nav-icon {
  background: var(--color-accent);
  color: var(--color-on-surface);
  border-color: var(--color-accent);
}

.step-nav-content h3 {
  margin: 0 0 var(--spacing-xs) 0;
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.step-nav-content p {
  margin: 0;
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  line-height: var(--line-height-body);
}

/* Быстрый предпросмотр и статистика */
.quick-preview,
.stats-preview {
  padding: var(--spacing-lg);
}

.quick-preview h4,
.stats-preview h4 {
  margin: 0 0 var(--spacing-md) 0;
  font-size: var(--font-size-base);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.preview-icon {
  font-size: var(--font-size-base);
}

.preview-badges {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.preview-badge {
  background: var(--color-backplate);
  padding: var(--spacing-sm);
  border-radius: var(--border-radius-md);
  font-size: var(--font-size-sm);
  border-left: 3px solid var(--color-primary);
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
}

.preview-badge:hover {
  transform: translateX(var(--spacing-xs));
  box-shadow: var(--shadow-level-1);
}

.stats-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-md);
}

.stat-item {
  text-align: center;
  padding: var(--spacing-md);
  background: var(--color-backplate);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
}

.stat-value {
  display: block;
  font-size: var(--font-size-xl);
  font-weight: var(--font-weight-heading);
  color: var(--color-primary);
  margin-bottom: var(--spacing-xs);
  font-family: var(--font-family-heading);
}

.stat-label {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  text-transform: lowercase;
}

/* Основное содержимое */
.wizard-main {
  background: var(--color-surface-elevated);
  border-radius: var(--border-radius-lg);
  box-shadow: var(--shadow-level-2);
  overflow: hidden;
  border: 1px solid var(--color-border);
}

.step-content {
  padding: var(--spacing-2xl);
  min-height: 600px;
}

.step-header {
  margin-bottom: var(--spacing-2xl);
  padding-bottom: var(--spacing-lg);
  border-bottom: 2px solid var(--color-border);
}

.step-header h2 {
  margin: 0 0 var(--spacing-md) 0;
  font-size: var(--font-size-xl);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.step-icon {
  font-size: var(--font-size-lg);
}

.step-header p {
  margin: 0;
  color: var(--color-on-surface-secondary);
  font-size: var(--font-size-base);
  line-height: var(--line-height-body);
}

/* Сетка форм */
.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-xl);
  margin-bottom: var(--spacing-xl);
}

.form-section {
  padding: var(--spacing-lg);
}

.form-section h3 {
  margin: 0 0 var(--spacing-lg) 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
  padding-bottom: var(--spacing-sm);
  border-bottom: 2px solid var(--color-primary);
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

/* Группы форм */
.form-group {
  margin-bottom: var(--spacing-lg);
}

.form-group label {
  display: block;
  margin-bottom: var(--spacing-sm);
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
  font-family: var(--font-family-body);
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
}

.label-icon {
  font-size: var(--font-size-base);
}

.form-group label.required::after {
  content: " *";
  color: var(--color-accent);
}

.input-container {
  padding: var(--spacing-xs);
}

.vintage-border {
  border: 1px solid var(--color-border);
  border-radius: var(--border-radius-md);
  background: var(--color-surface);
  box-shadow:
      inset 0 1px 2px color-mix(in srgb, var(--color-on-surface) 3%, transparent),
      0 2px 4px color-mix(in srgb, var(--color-neutral) 5%, transparent);
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
}

.vintage-border:focus-within {
  border-color: var(--color-primary);
  box-shadow:
      inset 0 1px 2px color-mix(in srgb, var(--color-on-surface) 3%, transparent),
      0 2px 8px color-mix(in srgb, var(--color-primary) 15%, transparent);
}

.form-group input,
.form-group textarea,
.form-group select {
  width: 100%;
  padding: var(--spacing-md);
  border: none;
  border-radius: var(--border-radius-sm);
  font-size: var(--font-size-base);
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
  font-family: var(--font-family-body);
  background: transparent;
  color: var(--color-on-surface);
}

.form-group input:focus,
.form-group textarea:focus,
.form-group select:focus {
  outline: none;
}

.form-group textarea {
  resize: vertical;
  min-height: 120px;
}

.form-group input.error {
  background: color-mix(in srgb, var(--color-accent) 10%, transparent);
}

.char-counter {
  text-align: right;
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  margin-top: var(--spacing-xs);
  font-style: italic;
}

.hint {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  margin-top: var(--spacing-xs);
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
  font-style: italic;
}

.hint-icon {
  font-size: var(--font-size-sm);
}

.hint code {
  background: var(--color-backplate);
  padding: 2px 6px;
  border-radius: var(--border-radius-sm);
  font-family: monospace;
  font-size: var(--font-size-sm);
}

/* Кнопки */

.btn-outline_left {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: var(--spacing-md) var(--spacing-lg);
  border: 2px solid;
  border-radius: var(--border-radius-md);
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-heading);
  text-decoration: none;
  cursor: pointer;
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
  font-family: var(--font-family-body);
}
.btn-outline {
  background: transparent;
  border-color: var(--color-border);
  color: var(--color-on-surface);
}

.btn-outline:hover:not(:disabled) {
  border-color: var(--color-primary);
  color: var(--color-primary);
}
/* Кнопка Сохранить задачу - Ретро стиль */
.wizard-navigation .btn-accent {
  color: var(--color-on-surface);
  border: 2px solid var(--color-accent);
  background: var(--color-accent);
  box-shadow:
      3px 3px 0 color-mix(in srgb, var(--color-secondary) 70%, transparent),
      var(--shadow-level-1);
  position: relative;
  font-weight: 600;
  font-family: var(--font-family-heading);
  letter-spacing: 0.5px;
  padding: var(--spacing-md) var(--spacing-2xl);
  transition: all 0.2s ease;
  text-transform: uppercase;
  font-size: var(--font-size-sm);
}

/* Эффект нажатия */
.wizard-navigation .btn-accent:hover:not(:disabled) {
  transform: translate(1px, 1px);
  box-shadow:
      2px 2px 0 color-mix(in srgb, var(--color-secondary) 70%, transparent),
      var(--shadow-level-1);
  background: color-mix(in srgb, var(--color-accent) 85%, black);
}

/* Активное состояние */
.wizard-navigation .btn-accent:active:not(:disabled) {
  transform: translate(3px, 3px);
  box-shadow:
      0px 0px 0 color-mix(in srgb, var(--color-secondary) 70%, transparent),
      var(--shadow-level-1);
}

/* Иконка дискеты */
.wizard-navigation .btn-accent .btn-icon {
  font-size: var(--font-size-base);
  margin-right: var(--spacing-xs);
  transition: transform 0.2s ease;
}

/* Анимация иконки */
.wizard-navigation .btn-accent:hover:not(:disabled) .btn-icon {
  transform: scale(1.1);
}

/* Фокус состояние */
.wizard-navigation .btn-accent:focus-visible {
  outline: 2px dashed var(--color-outline);
  outline-offset: 2px;
}

/* Disabled состояние */
.wizard-navigation .btn-accent:disabled {
  color: color-mix(in srgb, var(--color-on-surface) 50%, transparent);
  border: 2px solid color-mix(in srgb, var(--color-accent) 40%, transparent);
  background: color-mix(in srgb, var(--color-accent) 20%, transparent);
  box-shadow:
      2px 2px 0 color-mix(in srgb, var(--color-secondary) 20%, transparent),
      var(--shadow-level-1);
  transform: none;
  cursor: not-allowed;
}

/* Точки по углам в ретро-стиле */
.wizard-navigation .btn-accent::before,
.wizard-navigation .btn-accent::after {
  content: '';
  position: absolute;
  width: 6px;
  height: 6px;
  background: var(--color-secondary);
  border-radius: 50%;
}

.wizard-navigation .btn-accent::before {
  top: -2px;
  left: -2px;
}

.wizard-navigation .btn-accent::after {
  bottom: -2px;
  right: -2px;
}

/* Полоски по бокам */
.wizard-navigation .btn-accent {
  border-left: 4px solid color-mix(in srgb, var(--color-secondary) 60%, transparent);
  border-right: 4px solid color-mix(in srgb, var(--color-secondary) 60%, transparent);
}

/* Адаптивность */
@media (max-width: 768px) {
  .wizard-navigation .btn-accent {
    padding: var(--spacing-lg) var(--spacing-xl);
    font-size: var(--font-size-base);
  }
}
/* Кнопка Продолжить - Ретро стиль */
.wizard-navigation .btn-primary {
  color: var(--color-on-primary);
  border: 3px double var(--color-primary);
  background: var(--color-primary);
  box-shadow:
      4px 4px 0 color-mix(in srgb, var(--color-secondary) 80%, transparent),
      var(--shadow-level-1);
  position: relative;
  font-weight: 600;
  font-family: var(--font-family-heading);
  letter-spacing: 0.5px;
  padding: var(--spacing-md) var(--spacing-2xl);
  transition: all 0.2s ease;
  text-transform: uppercase;
  font-size: var(--font-size-sm);
}

/* Эффект нажатой кнопки */
.wizard-navigation .btn-primary:hover:not(:disabled) {
  transform: translate(2px, 2px);
  box-shadow:
      2px 2px 0 color-mix(in srgb, var(--color-secondary) 80%, transparent),
      var(--shadow-level-1);
  background: color-mix(in srgb, var(--color-primary) 90%, black);
}

/* Активное состояние */
.wizard-navigation .btn-primary:active:not(:disabled) {
  transform: translate(4px, 4px);
  box-shadow:
      0px 0px 0 color-mix(in srgb, var(--color-secondary) 80%, transparent),
      var(--shadow-level-1);
}

/* Иконка стрелки */
.wizard-navigation .btn-primary .btn-icon {
  font-size: var(--font-size-base);
  margin-left: var(--spacing-xs);
  transition: transform 0.2s ease;
}

/* Анимация стрелки */
.wizard-navigation .btn-primary:hover:not(:disabled) .btn-icon {
  transform: translateX(2px);
}

/* Фокус состояние */
.wizard-navigation .btn-primary:focus-visible {
  outline: 2px dashed var(--color-outline);
  outline-offset: 2px;
}

/* Disabled состояние */
.wizard-navigation .btn-primary:disabled {
  color: color-mix(in srgb, var(--color-on-primary) 60%, transparent);
  border: 3px double color-mix(in srgb, var(--color-primary) 50%, transparent);
  background: color-mix(in srgb, var(--color-primary) 30%, transparent);
  box-shadow:
      2px 2px 0 color-mix(in srgb, var(--color-secondary) 30%, transparent),
      var(--shadow-level-1);
  transform: none;
  cursor: not-allowed;
}

/* Ретро текстура */
.wizard-navigation .btn-primary {
  background-image:
      repeating-linear-gradient(
          45deg,
          transparent,
          transparent 2px,
          color-mix(in srgb, var(--color-on-primary) 5%, transparent) 2px,
          color-mix(in srgb, var(--color-on-primary) 5%, transparent) 4px
      );
}

/* Угловые акценты */
.wizard-navigation .btn-primary::before,
.wizard-navigation .btn-primary::after {
  content: '';
  position: absolute;
  width: 8px;
  height: 8px;
  background: var(--color-secondary);
}

.wizard-navigation .btn-primary::before {
  top: -3px;
  left: -3px;
  clip-path: polygon(0 0, 100% 0, 0 100%);
}

.wizard-navigation .btn-primary::after {
  bottom: -3px;
  right: -3px;
  clip-path: polygon(100% 0, 100% 100%, 0 100%);
}

/* Адаптивность */
@media (max-width: 768px) {
  .wizard-navigation .btn-primary {
    padding: var(--spacing-lg) var(--spacing-xl);
    font-size: var(--font-size-base);
  }
}

/* Навигация */
.wizard-navigation {
  padding: var(--spacing-lg);
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: var(--color-backplate);
  border-top: 1px solid var(--color-border);
}

.nav-left,
.nav-center,
.nav-right {
  flex: 1;
  display: flex;
  align-items: center;
}

.nav-center {
  justify-content: center;
}

.nav-right {
  justify-content: flex-end;
  gap: var(--spacing-md);
}

.step-info {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
  background: var(--color-surface);
  padding: var(--spacing-sm) var(--spacing-md);
  border-radius: var(--border-radius-full);
  border: 1px solid var(--color-border);
}

.step-current {
  font-weight: var(--font-weight-heading);
  color: var(--color-primary);
  font-size: var(--font-size-lg);
}

.step-separator {
  color: var(--color-on-surface-secondary);
}

.step-total {
  color: var(--color-on-surface-secondary);
}

/* Пример секции */
.example-section {
  padding: var(--spacing-lg);
  margin-top: var(--spacing-xl);
}

.example-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-lg);
}

.example-bad,
.example-good {
  padding: var(--spacing-md);
  border-radius: var(--border-radius-md);
}

.example-bad {
  background: color-mix(in srgb, var(--color-accent) 8%, transparent);
  border: 1px dashed var(--color-accent);
}

.example-good {
  background: color-mix(in srgb, var(--color-primary) 8%, transparent);
  border: 1px dashed var(--color-primary);
}

.example-bad h4,
.example-good h4 {
  margin: 0 0 var(--spacing-sm) 0;
  font-size: var(--font-size-base);
  font-family: var(--font-family-heading);
}

.example-bad p,
.example-good p {
  margin: 0;
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  line-height: var(--line-height-body);
}

.example-good code {
  background: var(--color-backplate);
  padding: 2px 6px;
  border-radius: var(--border-radius-sm);
  font-family: monospace;
  font-size: var(--font-size-sm);
}

/* Параметры */
.params-container {
  padding: var(--spacing-md);
}

.params-header {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 2fr 40px;
  gap: var(--spacing-sm);
  padding: var(--spacing-sm);
  background: var(--color-backplate);
  border-radius: var(--border-radius-sm);
  margin-bottom: var(--spacing-sm);
  font-weight: var(--font-weight-heading);
  font-size: var(--font-size-sm);
}

.param-row {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 2fr 40px;
  gap: var(--spacing-sm);
  margin-bottom: var(--spacing-sm);
  align-items: center;
}

/* Условия */
.conditions-list {
  padding: var(--spacing-md);
}

.condition-item {
  display: flex;
  gap: var(--spacing-sm);
  margin-bottom: var(--spacing-sm);
  align-items: center;
}

.condition-item input {
  flex: 1;
}

/* Предпросмотр */
.preview-section {
  margin-top: var(--spacing-xl);
  padding-top: var(--spacing-lg);
  border-top: 1px solid var(--color-border);
}

.code-preview {
  padding: var(--spacing-md);
  margin-bottom: var(--spacing-sm);
}

.code-preview pre {
  margin: 0;
  font-family: monospace;
  font-size: var(--font-size-sm);
  line-height: var(--line-height-body);
  color: var(--color-on-surface);
}

/* Языки */
.languages-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--spacing-md);
}

.language-option {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  cursor: pointer;
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
}

.language-option:hover {
  border-color: var(--color-primary);
}

.language-option.selected {
  border-color: var(--color-primary);
  background: color-mix(in srgb, var(--color-primary) 8%, transparent);
}

.lang-icon {
  font-size: var(--font-size-xl);
}

.lang-info {
  display: flex;
  flex-direction: column;
}

.lang-info strong {
  font-size: var(--font-size-base);
  font-family: var(--font-family-heading);
}

.lang-info span {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
}

/* Библиотеки */
.libraries-panel {
  padding: var(--spacing-md);
}

.libraries-search {
  padding: var(--spacing-md);
  border-bottom: 1px solid var(--color-border);
  background: var(--color-backplate);
}

.libraries-search input {
  width: 100%;
}

.libraries-list {
  max-height: 300px;
  overflow-y: auto;
}

.library-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-md);
  border-bottom: 1px solid var(--color-border);
  cursor: pointer;
  transition: background var(--animation-duration-standard) var(--animation-curve-primary);
}

.library-item:last-child {
  border-bottom: none;
}

.library-item:hover {
  background: var(--color-backplate);
}

.library-item.selected {
  background: color-mix(in srgb, var(--color-primary) 8%, transparent);
}

.lib-info strong {
  display: block;
  margin-bottom: var(--spacing-xs);
  font-family: var(--font-family-heading);
}

.lib-description {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  margin: var(--spacing-xs) 0 0 0;
}

.lib-compatibility {
  font-size: var(--font-size-sm);
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius-full);
  font-weight: var(--font-weight-heading);
}

.lib-compatibility.full {
  background: color-mix(in srgb, var(--color-primary) 15%, transparent);
  color: var(--color-primary);
}

.lib-compatibility.limited {
  background: color-mix(in srgb, var(--color-accent) 15%, transparent);
  color: var(--color-accent);
}

.selected-libraries {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-sm);
}

.selected-library {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
  background: var(--color-primary);
  color: var(--color-on-primary);
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius-full);
  font-size: var(--font-size-sm);
}

/* Настройки выполнения */
.execution-settings {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--spacing-lg);
}

/* Примеры */
.examples-container {
  padding: var(--spacing-md);
}

.example-item {
  padding: var(--spacing-md);
  margin-bottom: var(--spacing-md);
}

.example-item:last-child {
  margin-bottom: 0;
}

.example-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-md);
  padding-bottom: var(--spacing-sm);
  border-bottom: 1px solid var(--color-border);
}

.example-header h4 {
  margin: 0;
  font-family: var(--font-family-heading);
}

.example-content {
  display: grid;
  gap: var(--spacing-md);
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  font-weight: normal;
  cursor: pointer;
}

/* Тесты */
.tests-management {
  margin-bottom: var(--spacing-xl);
}

.tests-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-lg);
}

.tests-actions {
  display: flex;
  gap: var(--spacing-sm);
}

.tests-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.test-case {
  padding: var(--spacing-lg);
  border: 2px solid transparent;
}

.test-case.public {
  border-color: color-mix(in srgb, var(--color-primary) 30%, transparent);
}

.test-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--spacing-md);
}

.test-info h4 {
  margin: 0 0 var(--spacing-xs) 0;
  font-family: var(--font-family-heading);
}

.test-meta {
  display: flex;
  gap: var(--spacing-md);
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
}

.test-visibility {
  font-weight: var(--font-weight-heading);
}

.test-weight {
  font-weight: var(--font-weight-heading);
}

.test-actions {
  display: flex;
  gap: var(--spacing-sm);
}

.test-content {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: var(--spacing-lg);
}

.test-io {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-md);
}

.io-section label {
  display: block;
  margin-bottom: var(--spacing-sm);
  font-weight: var(--font-weight-heading);
}

.io-section textarea {
  width: 100%;
  min-height: 100px;
  resize: vertical;
}

.test-settings {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

/* Настройки тестирования */
.testing-settings {
  padding: var(--spacing-lg);
}

.testing-settings h3 {
  margin: 0 0 var(--spacing-lg) 0;
  font-family: var(--font-family-heading);
}

.settings-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: var(--spacing-md);
}

/* Теги */
.tags-input {
  padding: var(--spacing-sm);
}

.tags-list {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-xs);
  margin-bottom: var(--spacing-sm);
}

.tag {
  display: inline-flex;
  align-items: center;
  background: var(--color-secondary);
  color: var(--color-on-surface);
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius-full);
  font-size: var(--font-size-sm);
  gap: var(--spacing-xs);
}

.tag-remove {
  background: none;
  border: none;
  color: inherit;
  cursor: pointer;
  font-size: var(--font-size-base);
  line-height: 1;
  padding: 0;
  width: 16px;
  height: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Оценка сложности */
.difficulty-selector {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: var(--spacing-sm);
}

.difficulty-option {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: var(--spacing-md) var(--spacing-sm);
  cursor: pointer;
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
  text-align: center;
}

.difficulty-option:hover {
  border-color: var(--color-primary);
}

.difficulty-option.selected {
  border-color: var(--color-primary);
  background: color-mix(in srgb, var(--color-primary) 8%, transparent);
}

.diff-icon {
  font-size: var(--font-size-xl);
  margin-bottom: var(--spacing-xs);
}

.diff-label {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-heading);
}

/* Время выполнения */
.time-estimate {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: var(--spacing-sm);
}

.time-estimate input {
  width: 80px;
}

/* Адаптивность */
@media (max-width: 1200px) {
  .wizard-layout {
    grid-template-columns: 280px 1fr;
    gap: var(--spacing-lg);
  }
}

@media (max-width: 1024px) {
  .wizard-layout {
    grid-template-columns: 1fr;
  }

  .wizard-sidebar {
    position: static;
    order: 2;
  }

  .form-grid {
    grid-template-columns: 1fr;
  }

  .test-content {
    grid-template-columns: 1fr;
  }

  .test-io {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .container {
    padding: 0 var(--spacing-md);
  }

  .step-content {
    padding: var(--spacing-lg);
  }

  .builder-title {
    font-size: var(--font-size-xl);
    flex-direction: column;
    gap: var(--spacing-sm);
  }

  .wizard-navigation {
    flex-direction: column;
    gap: var(--spacing-lg);
    text-align: center;
  }

  .nav-left,
  .nav-center,
  .nav-right {
    justify-content: center;
    width: 100%;
  }

  .nav-right {
    flex-direction: column;
    gap: var(--spacing-sm);
  }

  .difficulty-selector {
    grid-template-columns: 1fr 1fr;
  }

  .languages-grid {
    grid-template-columns: 1fr;
  }

  .params-header,
  .param-row {
    grid-template-columns: 1fr;
    gap: var(--spacing-xs);
  }
}

@media (max-width: 480px) {
  .tests-header {
    flex-direction: column;
    gap: var(--spacing-md);
    align-items: stretch;
  }

  .tests-actions {
    justify-content: center;
  }

  .test-header {
    flex-direction: column;
    gap: var(--spacing-sm);
  }

  .test-actions {
    justify-content: flex-start;
  }

  .example-content {
    grid-template-columns: 1fr;
  }
}
</style>