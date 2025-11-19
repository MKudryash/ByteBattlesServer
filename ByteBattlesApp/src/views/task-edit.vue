<template>
  <div class="task-edit-container">
    <app-navigation></app-navigation>

    <div class="task-edit-wrapper">
      <DangerousHTML
          html="<style>
  .task-edit-container {
    min-height: 100vh;
    background: var(--color-surface);
    padding: var(--spacing-2xl) 0;
  }

  .task-edit-container::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-image:
      radial-gradient(circle at 60% 40%, color-mix(in srgb, var(--color-primary) 4%, transparent) 0%, transparent 50%),
      repeating-linear-gradient(
        90deg,
        transparent,
        transparent 2px,
        color-mix(in srgb, var(--color-border) 2%, transparent) 2px,
        color-mix(in srgb, var(--color-border) 2%, transparent) 4px
      );
    pointer-events: none;
    z-index: 1;
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

  @keyframes slideInFromLeft {
    from {
      opacity: 0;
      transform: translateX(-20px);
    }
    to {
      opacity: 1;
      transform: translateX(0);
    }
  }

  .edit-section {
    animation: slideInFromLeft 0.5s var(--animation-curve-primary);
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

      <section class="task-edit-section" role="main" aria-label="Редактирование задачи">
        <div class="container">
          <!-- Хлебные крошки -->
          <nav class="breadcrumbs" aria-label="Навигация">
            <ol class="breadcrumbs-list">
              <li class="breadcrumb-item">
                <router-link to="/tasks" class="breadcrumb-link">Задачи</router-link>
              </li>
              <li class="breadcrumb-separator">/</li>
              <li class="breadcrumb-item">
                <router-link :to="`/tasks/${taskId}`" class="breadcrumb-link">{{ taskData.title || 'Задача' }}</router-link>
              </li>
              <li class="breadcrumb-separator">/</li>
              <li class="breadcrumb-item">
                <span class="breadcrumb-current">Редактирование</span>
              </li>
            </ol>
          </nav>

          <div class="page-header">
            <div class="title-section">
              <h1 class="page-title">
                {{ isEditMode ? 'Редактирование задачи' : 'Создание новой задачи' }}
              </h1>
              <p class="page-subtitle">
                {{ isEditMode ? 'Внесите изменения в задачу и сохраните их' : 'Создайте новую учебную задачу для студентов' }}
              </p>
            </div>

            <!-- Статус сохранения -->
            <div class="save-status retro-card" v-if="saveStatus">
              <div class="status-content" :class="saveStatus.type">
                <span class="status-icon">{{ saveStatus.icon }}</span>
                <span class="status-text">{{ saveStatus.message }}</span>
              </div>
            </div>
          </div>

          <!-- Индикатор загрузки -->
          <div class="loading-state retro-card" v-if="isLoading">
            <div class="loading-icon">⏳</div>
            <h3>Загрузка задачи...</h3>
            <p>Пожалуйста, подождите</p>
          </div>

          <div class="edit-layout" v-else>
            <!-- Боковая панель -->
            <aside class="edit-sidebar" role="complementary" aria-label="Быстрая навигация">
              <!-- Навигация по разделам -->
              <div class="edit-nav retro-card">
                <h3 class="nav-title">
                  Разделы редактирования
                </h3>
                <nav class="nav-list">
                  <button
                      v-for="section in editSections"
                      :key="section.id"
                      :class="['nav-item', { 'active': activeSection === section.id }]"
                      @click="activeSection = section.id"
                  >
                    <span class="nav-item-icon">{{ section.icon }}</span>
                    <span class="nav-item-text">{{ section.name }}</span>
                    <span class="nav-item-badge" v-if="section.hasErrors && getSectionErrors(section.id)">
                      {{ getSectionErrors(section.id) }}
                    </span>
                  </button>
                </nav>
              </div>

              <!-- Предпросмотр задачи -->
              <div class="task-preview retro-card">
                <h3 class="preview-title">
                  Быстрый предпросмотр
                </h3>
                <div class="preview-content">
                  <div class="preview-field">
                    <label>Название:</label>
                    <span class="preview-value">{{ taskData.title || 'Не указано' }}</span>
                  </div>
                  <div class="preview-field">
                    <label>Сложность:</label>
                    <span class="preview-value" :class="taskData.difficulty">
                      {{ getDifficultyLabel(taskData.difficulty) || 'Не указана' }}
                    </span>
                  </div>
                  <div class="preview-field">
                    <label>Язык:</label>
                    <span class="preview-value">{{ getLanguageName(taskData.language) || 'Не выбран' }}</span>
                  </div>
                  <div class="preview-field">
                    <label>Теги:</label>
                    <span class="preview-value">{{ taskData.tags.length ? taskData.tags.join(', ') : 'Нет тегов' }}</span>
                  </div>
                </div>
              </div>

              <!-- Действия -->
              <div class="edit-actions retro-card">
                <h3 class="actions-title">Действия</h3>
                <div class="actions-list">
                  <button @click="saveTask" class="btn-primary full-width" :disabled="isSaving">
                    {{ isSaving ? 'Сохранение...' : (isEditMode ? 'Обновить задачу' : 'Создать задачу') }}
                  </button>
                  <button @click="saveDraft" class="btn-outline full-width" :disabled="isSaving">
                    Сохранить черновик
                  </button>
                  <button @click="previewTask" class="btn-text full-width">
                    Предпросмотр
                  </button>
                  <button @click="duplicateTask" class="btn-text full-width" v-if="isEditMode">
                    <span class="btn-icon">📋</span>
                    Дублировать задачу
                  </button>
                  <button @click="deleteTask" class="btn-text full-width delete-btn" v-if="isEditMode">
                    <span class="btn-icon">🗑️</span>
                    Удалить задачу
                  </button>
                </div>
              </div>
            </aside>

            <!-- Основное содержимое -->
            <main class="edit-main" role="region" :aria-label="`Редактирование: ${getActiveSectionName()}`">
              <!-- Раздел: Основная информация -->
              <div v-if="activeSection === 'basic'" class="edit-section">
                <div class="section-header">
                  <h2>
                    Основная информация
                  </h2>
                  <p>Задайте название, описание и основные параметры задачи</p>
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
                            :class="{ 'error': errors.title }"
                        >
                      </div>
                      <div class="char-counter">{{ taskData.title.length }}/100</div>
                      <div class="error-message" v-if="errors.title">{{ errors.title }}</div>
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
                            :class="{ 'error': errors.description }"
                        ></textarea>
                      </div>
                      <div class="hint">
                        <span class="hint-icon">💡</span>
                        Используйте Markdown для форматирования текста
                      </div>
                      <div class="error-message" v-if="errors.description">{{ errors.description }}</div>
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
                            :class="['difficulty-option vintage-border', {
                            'selected': taskData.difficulty === diff.value,
                            'error': errors.difficulty
                          }]"
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
                      <div class="error-message" v-if="errors.difficulty">{{ errors.difficulty }}</div>
                    </div>

                    <div class="form-group">
                      <label for="task-tags">
                        Теги
                      </label>
                      <div class="tags-input vintage-border" :class="{ 'error': errors.tags }">
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
                      <div class="hint">
                        Теги помогают студентам быстрее находить нужные задачи
                      </div>
                      <div class="error-message" v-if="errors.tags">{{ errors.tags }}</div>
                    </div>

                    <div class="form-group">
                      <label for="time-estimate">
                        Примерное время выполнения (минут)
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
              </div>

              <!-- Раздел: Сигнатура функции -->
              <div v-if="activeSection === 'signature'" class="edit-section">
                <div class="section-header">
                  <h2>
                    Сигнатура функции
                  </h2>
                  <p>Определите функцию, которую должен реализовать студент</p>
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
                          :class="{ 'error': errors.functionName }"
                      >
                    </div>
                    <div class="error-message" v-if="errors.functionName">{{ errors.functionName }}</div>
                  </div>

                  <div class="form-group">
                    <label class="required">
                      Параметры функции
                    </label>
                    <div class="params-container vintage-border" :class="{ 'error': errors.parameters }">
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
                    <div class="error-message" v-if="errors.parameters">{{ errors.parameters }}</div>
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
                  <h3>Предпросмотр сигнатуры</h3>
                  <div class="code-preview vintage-border">
                    <pre><code>{{ generateFunctionSignature() }}</code></pre>
                  </div>
                  <div class="hint">
                    Эта сигнатура будет автоматически подставлена в шаблон кода
                  </div>
                </div>
              </div>

              <!-- Раздел: Окружение -->
              <div v-if="activeSection === 'environment'" class="edit-section">
                <div class="section-header">
                  <h2>
                    <span class="section-icon">⚙️</span>
                    Окружение выполнения
                  </h2>
                  <p>Настройте язык программирования и необходимые зависимости</p>
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
                            :class="['language-option vintage-border', {
                            'selected': taskData.language === lang.id,
                            'error': errors.language
                          }]"
                        >
                          <input
                              type="radio"
                              v-model="taskData.language"
                              :value="lang.id"
                              hidden
                              @change="onLanguageChange"
                          >
                          <div class="lang-icon">{{ lang.icon }}</div>
                          <div class="lang-info">
                            <strong>{{ lang.name }}</strong>
                            <span>{{ lang.version }}</span>
                          </div>
                        </label>
                      </div>
                      <div class="error-message" v-if="errors.language">{{ errors.language }}</div>
                    </div>

                    <div class="form-group" v-if="taskData.language">
                      <label for="code-template">
                        Шаблон функции
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
                        Используйте <code>{{ function_signature }}</code> для автоматической вставки сигнатуры функции
                      </div>
                    </div>

                    <div class="form-group" v-if="taskData.language">
                      <label for="main-template">
                        Шаблон main функции
                      </label>
                      <div class="input-container vintage-border">
                        <textarea
                            id="main-template"
                            v-model="taskData.mainTemplate"
                            rows="8"
                            placeholder="Код, который будет выполняться при запуске программы..."
                        ></textarea>
                      </div>
                      <div class="hint">
                        Используйте <code>{{ function_call }}</code> для вызова студенческой функции
                      </div>
                    </div>
                  </div>

                  <div class="form-section retro-card">
                    <h3>Библиотеки и зависимости</h3>

                    <div class="form-group">
                      <label>Доступные библиотеки</label>
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
                      <label>Выбранные библиотеки</label>
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
              </div>

              <!-- Раздел: Тестирование -->
              <div v-if="activeSection === 'testing'" class="edit-section">
                <div class="section-header">
                  <h2>
                    <span class="section-icon">✅</span>
                    Система тестирования
                  </h2>
                  <p>Добавьте тесты для автоматической проверки решений студентов</p>
                </div>

                <div class="tests-management">
                  <div class="tests-header">
                    <h3>Тестовые случаи</h3>
                    <div class="tests-actions">
                      <button @click="addTest" class="btn-outline btn-sm">
                        <span class="btn-icon">+</span>
                        Добавить тест
                      </button>
                    </div>
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
                          </div>
                        </div>
                        <div class="test-actions">
                          <button @click="toggleTestVisibility(index)" class="btn-sm btn-outline">
                            {{ test.isPublic ? 'Скрыть' : 'Показать' }}
                          </button>
                          <button @click="removeTest(index)" class="btn-remove" :disabled="taskData.tests.length === 1">
                            Удалить
                          </button>
                        </div>
                      </div>

                      <div class="test-content">
                        <div class="test-io">
                          <div class="form-group">
                            <label class="required">Входные данные</label>
                            <div class="input-container vintage-border">
                              <textarea
                                  v-model="test.input"
                                  rows="3"
                                  placeholder="Входные данные для теста"
                                  :class="{ 'error': !test.input && testSubmitted }"
                              ></textarea>
                            </div>
                          </div>
                          <div class="form-group">
                            <label class="required">Ожидаемый вывод</label>
                            <div class="input-container vintage-border">
                              <textarea
                                  v-model="test.expectedOutput"
                                  rows="3"
                                  placeholder="Ожидаемый результат"
                                  :class="{ 'error': !test.expectedOutput && testSubmitted }"
                              ></textarea>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </main>
          </div>
        </div>
      </section>
    </div>

    <app-footer></app-footer>

    <!-- Диалог подтверждения удаления -->
    <div v-if="showDeleteDialog" class="dialog-overlay">
      <div class="dialog retro-card">
        <h3 class="dialog-title">Удаление задачи</h3>
        <p class="dialog-message">Вы уверены, что хотите удалить задачу "{{ taskData.title }}"? Это действие нельзя отменить.</p>
        <div class="dialog-actions">
          <button @click="showDeleteDialog = false" class="btn-outline">Отмена</button>
          <button @click="confirmDelete" class="btn-primary delete-btn">Удалить</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import DangerousHTML from 'dangerous-html/vue'
import AppNavigation from '../components/navigation'
import AppFooter from '../components/footer'
import { languageAPI, taskAPI } from '../api/task.js'

export default {
  name: 'TaskEdit',
  components: {
    AppNavigation,
    DangerousHTML,
    AppFooter,
  },
  props: {
    taskId: {
      type: String,
      default: null
    }
  },
  data() {
    return {
      activeSection: 'basic',
      isSaving: false,
      isLoading: false,
      saveStatus: null,
      showDeleteDialog: false,
      paramSubmitted: false,
      testSubmitted: false,
      librarySearch: '',
      newTag: '',
      error: null,

      editSections: [
        { id: 'basic', name: 'Основная информация', icon: '📝', hasErrors: true },
        { id: 'signature', name: 'Сигнатура функции', icon: '🔧', hasErrors: true },
        { id: 'environment', name: 'Окружение', icon: '⚙️', hasErrors: true },
        { id: 'testing', name: 'Тестирование', icon: '✅', hasErrors: true }
      ],

      taskData: {
        title: '',
        description: '',
        category: '',
        difficulty: 'Medium',
        tags: [],
        timeEstimate: 30,

        functionName: '',
        parameters: [{ name: '', type: 'int', defaultValue: '', description: '' }],
        returnType: 'void',

        language: '',
        codeTemplate: '',
        mainTemplate: '',
        libraries: [],
        timeLimit: 10,
        memoryLimit: 256,
        outputLimit: 64,

        tests: [{
          input: '',
          expectedOutput: '',
          isPublic: true,
        }]
      },

      errors: {},

      difficultyLevels: [
        { value: 'Easy', label: 'Начинающий', icon: '🌱' },
        { value: 'Medium', label: 'Средний', icon: '🎯' },
        { value: 'Hard', label: 'Продвинутый', icon: '🚀' }
      ],

      availableLanguages: [],
      availableLibraries: []
    }
  },
  computed: {
    isEditMode() {
      return !!this.taskId
    },

    filteredLibraries() {
      if (!this.librarySearch) return this.availableLibraries
      return this.availableLibraries.filter(lib =>
          lib.name.toLowerCase().includes(this.librarySearch.toLowerCase()) ||
          lib.description.toLowerCase().includes(this.librarySearch.toLowerCase())
      )
    }
  },
  async mounted() {
    await this.loadLanguages();
    if (this.isEditMode) {
      await this.loadTask();
    } else {
      this.setDefaultTemplates();
    }
    if (this.taskData.language) {
      this.$nextTick(() => {
        this.updateCodeTemplates();
      });
    }

    this.validateAllSections();
  },
  watch: {
    taskData: {
      deep: true,
      handler() {
        // Убедимся что parameters всегда массив
        if (!Array.isArray(this.taskData.parameters)) {
          this.taskData.parameters = [{ name: '', type: 'int', defaultValue: '', description: '' }]
        }
        this.validateAllSections()
      }
    },

    // Добавьте вотчер для языка
    'taskData.language': {
      handler(newLangId) {
        if (newLangId) {
          console.log('Language changed in edit mode:', newLangId);
          this.onLanguageChange();
        }
      },
      immediate: true
    },

    // Следим за изменениями в сигнатуре
    'taskData.functionName': function() {
      if (this.taskData.language) {
        this.$nextTick(() => {
          this.updateCodeTemplates();
        });
      }
    },

    'taskData.parameters': {
      handler() {
        if (this.taskData.language) {
          this.$nextTick(() => {
            this.updateCodeTemplates();
          });
        }
      },
      deep: true
    },

    'taskData.returnType': function() {
      if (this.taskData.language) {
        this.$nextTick(() => {
          this.updateCodeTemplates();
        });
      }
    }
  },
  methods: {
    async loadLanguages() {
      this.isLoading = true
      try {
        const languages = await languageAPI.getAll()
        this.availableLanguages = languages.map(lang => ({
          id: lang.id,
          name: lang.title || 'Unknown Language',
          version: lang.version || '1.0',
          icon: this.getLanguageIcon(lang.title),
          patternFunction: lang.patternFunction,
          patternMain: lang.patternMain,
          libraries: lang.libraries || []
        }))
      } catch (error) {
        console.error('Ошибка при загрузке языков:', error)
        this.showSaveStatus('error', 'Не удалось загрузить список языков')
      } finally {
        this.isLoading = false
      }
    },

    async loadTask() {
      this.isLoading = true
      try {
        const task = await taskAPI.getById(this.taskId)

        // Получаем languageId из taskLanguages, если он не приходит напрямую
        let languageId = task.languageId;
        if (!languageId && task.taskLanguages && task.taskLanguages.length > 0) {
          languageId = task.taskLanguages[0].languageId;
          console.log('Language ID from taskLanguages:', languageId);
        }

        // Преобразуем параметры из строки в массив объектов
        let parameters = []
        if (task.parameters && typeof task.parameters === 'string') {
          // Парсим строку параметров вида "t: int, t: int"
          parameters = this.parseParameters(task.parameters)
        } else if (Array.isArray(task.parameters)) {
          parameters = task.parameters
        } else {
          parameters = [{ name: '', type: 'int', defaultValue: '', description: '' }]
        }

        this.taskData = {
          title: task.title,
          description: task.description,
          difficulty: task.difficulty,
          category: task.category || '',
          tags: task.tags || [],
          timeEstimate: task.timeEstimate || 30,

          functionName: task.functionName,
          parameters: parameters,
          returnType: task.returnType || 'void',

          language: languageId || '', // Используем languageId из taskLanguages
          codeTemplate: task.patternFunction || '',
          mainTemplate: task.patternMain || '',
          libraries: task.libraries || [],

          tests: task.tests || [{
            input: '',
            expectedOutput: '',
            isPublic: true
          }]
        }

        console.log('Loaded task data:', this.taskData);

        // Загружаем тестовые случаи
        if (this.isEditMode) {
          const testCases = await taskAPI.getTestCases(this.taskId)
          if (testCases && testCases.length > 0) {
            this.taskData.tests = testCases.map(test => ({
              input: test.input,
              expectedOutput: test.output,
              isPublic: test.isPublic || false,
            }))
          }
        }

      } catch (error) {
        console.error('Ошибка загрузки задачи:', error)
        this.showSaveStatus('error', 'Ошибка загрузки задачи')
      } finally {
        this.isLoading = false
      }
    },

// Добавьте метод для парсинга параметров
    parseParameters(parametersString) {
      if (!parametersString || typeof parametersString !== 'string') {
        return [{ name: '', type: 'int', defaultValue: '', description: '' }]
      }

      try {
        // Разделяем параметры по запятой
        const paramStrings = parametersString.split(',').map(p => p.trim()).filter(p => p)

        return paramStrings.map(paramStr => {
          // Парсим параметры вида "name: type" или "name: type = defaultValue"
          const parts = paramStr.split(':').map(p => p.trim())
          if (parts.length < 2) {
            return { name: parts[0] || '', type: 'int', defaultValue: '', description: '' }
          }

          const name = parts[0]
          let typeAndDefault = parts[1]

          // Проверяем есть ли значение по умолчанию
          let type = typeAndDefault
          let defaultValue = ''

          if (typeAndDefault.includes('=')) {
            const typeDefaultParts = typeAndDefault.split('=').map(p => p.trim())
            type = typeDefaultParts[0]
            defaultValue = typeDefaultParts[1] || ''
          }

          return {
            name: name,
            type: type || 'int',
            defaultValue: defaultValue,
            description: ''
          }
        })
      } catch (error) {
        console.error('Ошибка парсинга параметров:', error)
        return [{ name: '', type: 'int', defaultValue: '', description: '' }]
      }
    },

    setDefaultTemplates() {
      // Установим базовые шаблоны при создании новой задачи
      this.taskData.codeTemplate = `// Ваша реализация здесь\n// Используйте готовую сигнатуру функции`
      this.taskData.mainTemplate = `// Точка входа программы\n// Здесь можно протестировать вашу функцию`
    },
    getCurrentUser() {
      // Замените на ваш способ получения текущего пользователя
      // Например, из Vuex store, localStorage, или другого места
      return JSON.parse(localStorage.getItem("user")).firstName || 'default_user';
    },
    async saveTask() {
      this.paramSubmitted = true
      this.testSubmitted = true
      this.validateAllSections()

      if (Object.keys(this.errors).length > 0) {
        this.showSaveStatus('error', 'Исправьте ошибки перед сохранением')
        const errorSection = this.editSections.find(s => s.hasErrors)
        if (errorSection) this.activeSection = errorSection.id
        return
      }

      this.isSaving = true
      try {
        console.log('Отправляемые данные:', this.taskData)
        const currentUser = this.getCurrentUser();
        // Подготавливаем данные для отправки
        const taskToSave = {
          title: this.taskData.title,
          description: this.taskData.description,
          difficulty: this.taskData.difficulty,
          // category: this.taskData.category,
          author: currentUser,
          functionName: this.taskData.functionName,
           parameters: this.formatInputParameters(),
           returnType: this.taskData.returnType,

          languageId: this.formatLanguageIds(),
          patternFunction: this.taskData.codeTemplate,
          patternMain: this.taskData.mainTemplate
        }

        console.log('Данные для сохранения:', taskToSave)

        let response
        if (this.isEditMode) {
          // Обновление существующей задачи
          response = await taskAPI.update({
            ...taskToSave,
            id: this.taskId
          })
          console.log('Задача обновлена:', response)
          this.showSaveStatus('success', 'Задача успешно обновлена')
        } else {
          // Создание новой задачи
          response = await taskAPI.create(taskToSave)
          console.log('Задача создана:', response)
          this.showSaveStatus('success', 'Задача успешно создана')

          // Сохраняем тестовые случаи для новой задачи
          if (response && response.id) {
            await this.saveTestCases(response.id)
          }

          // Перенаправляем на страницу задачи
          setTimeout(() => {
            this.$router.push(`/tasks/${response.id}`)
          }, 1500)
        }

      } catch (error) {
        console.error('Полная ошибка сохранения:', error)
        console.error('Детали ошибки:', error.response?.data)
        console.error('Статус ошибки:', error.response?.status)

        let errorMessage = 'Ошибка при сохранении задачи'
        if (error.response?.data?.detail) {
          errorMessage = error.response.data.detail
        } else if (error.response?.data?.title) {
          errorMessage = error.response.data.title
        }

        this.showSaveStatus('error', errorMessage)
      } finally {
        this.isSaving = false
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
        }

        if (testCasesDto.testCases.length > 0) {
          await taskAPI.createTestCases(taskId, testCasesDto)
          console.log('Тестовые случаи успешно сохранены')
        }
      } catch (error) {
        console.error('Ошибка при сохранении тестовых случаев:', error)
        throw error
      }
    },

    async saveDraft() {
      this.isSaving = true
      try {
        // Здесь можно сохранить черновик в localStorage или отправить на сервер
        localStorage.setItem('taskDraft', JSON.stringify(this.taskData))
        this.showSaveStatus('success', 'Черновик сохранен')
      } catch (error) {
        this.showSaveStatus('error', 'Ошибка сохранения черновика')
      } finally {
        this.isSaving = false
      }
    },

    previewTask() {
      const previewData = {
        ...this.taskData,
        id: this.isEditMode ? this.taskId : 'preview'
      }
      localStorage.setItem('taskPreview', JSON.stringify(previewData))
      window.open('/task-preview', '_blank')
    },

    duplicateTask() {
      this.taskData.title = `${this.taskData.title} (копия)`
      this.taskId = null
      this.showSaveStatus('info', 'Создается копия задачи')
    },

    deleteTask() {
      this.showDeleteDialog = true
    },

    async confirmDelete() {
      try {
        await taskAPI.delete(this.taskId)
        this.showDeleteDialog = false
        this.showSaveStatus('success', 'Задача удалена')
        setTimeout(() => {
          this.$router.push('/tasks')
        }, 1000)
      } catch (error) {
        this.showSaveStatus('error', 'Ошибка при удалении задачи')
      }
    },

    // Валидация
    validateAllSections() {
      this.errors = {}

      if (!this.taskData.title?.trim()) {
        this.errors.title = 'Название задачи обязательно'
      }
      if (!this.taskData.description?.trim()) {
        this.errors.description = 'Описание задачи обязательно'
      }
      if (!this.taskData.difficulty) {
        this.errors.difficulty = 'Укажите сложность задачи'
      }
      if (!this.taskData.functionName?.trim()) {
        this.errors.functionName = 'Имя функции обязательно'
      }
      if (this.taskData.parameters.some(p => !p.name.trim())) {
        this.errors.parameters = 'Все параметры должны иметь имя'
      }
      if (!this.taskData.language) {
        this.errors.language = 'Выберите язык программирования'
      }

      this.updateSectionErrors()
    },

    updateSectionErrors() {
      this.editSections.forEach(section => {
        switch (section.id) {
          case 'basic':
            section.hasErrors = !!this.errors.title || !!this.errors.description || !!this.errors.difficulty
            break
          case 'signature':
            section.hasErrors = !!this.errors.functionName || !!this.errors.parameters
            break
          case 'environment':
            section.hasErrors = !!this.errors.language
            break
          case 'testing':
            section.hasErrors = this.taskData.tests.length === 0 ||
                this.taskData.tests.some(t => !t.input.trim() || !t.expectedOutput.trim())
            break
        }
      })
    },

    getSectionErrors(sectionId) {
      switch (sectionId) {
        case 'basic':
          return Object.keys(this.errors).filter(k => ['title', 'description', 'difficulty'].includes(k)).length
        case 'signature':
          return Object.keys(this.errors).filter(k => ['functionName', 'parameters'].includes(k)).length
        case 'environment':
          return this.errors.language ? 1 : 0
        case 'testing':
          const testErrors = this.taskData.tests.filter(t => !t.input.trim() || !t.expectedOutput.trim()).length
          return testErrors + (this.taskData.tests.length === 0 ? 1 : 0)
        default:
          return 0
      }
    },

    // Вспомогательные методы
    showSaveStatus(type, message) {
      const icons = {
        success: '✅',
        error: '❌',
        info: 'ℹ️',
        warning: '⚠️'
      }

      this.saveStatus = {
        type,
        icon: icons[type] || 'ℹ️',
        message
      }

      setTimeout(() => {
        this.saveStatus = null
      }, 5000)
    },

    getActiveSectionName() {
      const section = this.editSections.find(s => s.id === this.activeSection)
      return section ? section.name : ''
    },

    getLanguageName(langId) {
      const lang = this.availableLanguages.find(l => l.id === langId);
      return lang ? lang.name : (langId || 'Не выбран');
    },

    getDifficultyLabel(difficulty) {
      const diff = this.difficultyLevels.find(d => d.value === difficulty)
      return diff ? diff.label : difficulty
    },

    getLanguageIcon(languageName) {
      const iconMap = {
        'python': '🐍',
        'java': '☕',
        'javascript': '📜',
        'typescript': '🔷',
        'cpp': '⚡',
        'csharp': '🎵'
      }
      const lowerName = (languageName || '').toLowerCase()
      return iconMap[lowerName] || '💻'
    },

    // Методы для работы с параметрами
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

    formatInputParameters() {
      return this.taskData.parameters
          .filter(param => param.name.trim())
          .map(param => {
            let paramStr = `${param.name}: ${param.type}`
            if (param.defaultValue) {
              paramStr += ` = ${param.defaultValue}`
            }
            if (param.description) {
              paramStr += ` // ${param.description}`
            }
            return paramStr
          })
          .join(', ')
    },

    formatLanguageIds() {
      return this.taskData.language ? [this.taskData.language] : []
    },

    // Методы для работы с тегами
    addTag() {
      if (this.newTag.trim() && !this.taskData.tags.includes(this.newTag.trim())) {
        this.taskData.tags.push(this.newTag.trim())
        this.newTag = ''
      }
    },

    removeTag(index) {
      this.taskData.tags.splice(index, 1)
    },

    // Методы для работы с библиотеками
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

    onLanguageChange() {
      console.log('Language changed to:', this.taskData.language);

      // Загружаем библиотеки для выбранного языка
      this.loadLibrariesForLanguage(this.taskData.language);

      // Обновляем шаблоны кода с правильной сигнатурой
      this.updateCodeTemplates();

      // Очищаем выбранные библиотеки при смене языка
      this.taskData.libraries = [];
    },

    async loadLibrariesForLanguage(languageId) {
      if (!languageId) {
        this.availableLibraries = []
        return
      }

      try {
        const language = this.availableLanguages.find(lang => lang.id === languageId)
        if (language && language.libraries) {
          this.availableLibraries = language.libraries.map(lib => ({
            id: lib.id,
            name: lib.name,
            version: lib.version,
            description: lib.description,
            compatibility: 'full'
          }))
        }
      } catch (error) {
        console.error('Error loading libraries:', error)
      }
    },

    updateCodeTemplates() {
      if (!this.taskData.language) return;

      const language = this.availableLanguages.find(lang => lang.id === this.taskData.language);
      if (!language) return;

      console.log('Updating templates for language:', language.name);

      // Генерируем правильную сигнатуру для выбранного языка
      const functionSignature = this.generateLanguageSpecificSignature();

      // ВСЕГДА обновляем шаблон функции при смене языка
      if (language.patternFunction) {
        this.taskData.codeTemplate = language.patternFunction.replace('{{function_signature}}', functionSignature);
        console.log('Updated code template with language pattern');
      } else {
        // Базовый шаблон, если нет готового
        console.log('Updated code template with generated template');
      }

      // ВСЕГДА обновляем шаблон main при смене языка
      if (language.patternMain) {
        this.taskData.mainTemplate = language.patternMain;
        console.log('Updated main template with language pattern');
      } else {
        console.log('Updated main template with default template');
      }
    },
    generateLanguageSpecificSignature() {
      if (!this.taskData.functionName || !this.taskData.language) {
        return this.generateFunctionSignature(); // fallback
      }

      const language = this.availableLanguages.find(lang => lang.id === this.taskData.language);
      if (!language) return this.generateFunctionSignature();

      const params = this.taskData.parameters
          .filter(p => p.name && p.name.trim())
          .map(p => {
            let paramStr = p.name;

            // Добавляем типы для языков, которые их поддерживают
            if (p.type && this.supportsTypeAnnotations(this.taskData.language)) {
              paramStr = this.formatParameterWithType(p.name, p.type, this.taskData.language);
            }

            if (p.defaultValue && p.defaultValue.trim()) {
              paramStr += this.formatDefaultValue(p.defaultValue, this.taskData.language);
            }

            return paramStr;
          })
          .join(', ');

      return this.formatFunctionSignature(this.taskData.functionName, params, this.taskData.returnType, this.taskData.language);
    },
    // Проверка поддержки аннотаций типов для языка
    supportsTypeAnnotations(languageId) {
      const language = this.availableLanguages.find(lang => lang.id === languageId);
      if (!language) return false;

      const typedLanguages = ['cs', 'cpp', 'java']; // C#, C++, Java
      const languageName = language.name?.toLowerCase() || '';

      return typedLanguages.some(lang => languageName.includes(lang));
    },

// Форматирование параметра с типом
    formatParameterWithType(paramName, paramType, languageId) {
      const language = this.availableLanguages.find(lang => lang.id === languageId);
      const languageName = language?.name?.toLowerCase() || '';

      if (languageName.includes('csharp') || languageName.includes('c#')) {
        return `${this.mapTypeToLanguage(paramType, languageId)} ${paramName}`;
      } else if (languageName.includes('cpp') || languageName.includes('c++')) {
        return `${this.mapTypeToLanguage(paramType, languageId)} ${paramName}`;
      } else if (languageName.includes('java')) {
        return `${this.mapTypeToLanguage(paramType, languageId)} ${paramName}`;
      } else if (languageName.includes('python')) {
        return `${paramName}: ${this.mapTypeToLanguage(paramType, languageId)}`;
      }

      return paramName;
    },

// Форматирование значения по умолчанию
    formatDefaultValue(defaultValue, languageId) {
      const language = this.availableLanguages.find(lang => lang.id === languageId);
      const languageName = language?.name?.toLowerCase() || '';

      if (languageName.includes('python')) {
        return ` = ${defaultValue}`;
      } else {
        return ` = ${defaultValue}`;
      }
    },

// Форматирование полной сигнатуры функции
    formatFunctionSignature(functionName, params, returnType, languageId) {
      const language = this.availableLanguages.find(lang => lang.id === languageId);
      const languageName = language?.name?.toLowerCase() || '';

      let signature = `${functionName}(${params})`;

      // Добавляем возвращаемый тип
      if (returnType && returnType !== 'void') {
        if (languageName.includes('python')) {
          signature += ` -> ${this.mapTypeToLanguage(returnType, languageId)}`;
        } else if (languageName.includes('csharp') || languageName.includes('c#')) {
          signature = `public static ${this.mapTypeToLanguage(returnType, languageId)} ${signature}`;
        } else if (languageName.includes('cpp') || languageName.includes('c++')) {
          signature = `${this.mapTypeToLanguage(returnType, languageId)} ${signature}`;
        } else if (languageName.includes('java')) {
          signature = `public static ${this.mapTypeToLanguage(returnType, languageId)} ${signature}`;
        }
      } else if (returnType === 'void') {
        if (languageName.includes('csharp') || languageName.includes('c#')) {
          signature = `public static void ${signature}`;
        } else if (languageName.includes('java')) {
          signature = `public static void ${signature}`;
        } else if (languageName.includes('cpp') || languageName.includes('c++')) {
          signature = `void ${signature}`;
        }
      }

      return signature;
    },

// Маппинг типов на конкретный язык
    mapTypeToLanguage(type, languageId) {
      const language = this.availableLanguages.find(lang => lang.id === languageId);
      const languageName = language?.name?.toLowerCase() || '';

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

      // Определяем, какую карту типов использовать
      let langMap;
      if (languageName.includes('python')) {
        langMap = typeMap.python;
      } else if (languageName.includes('java')) {
        langMap = typeMap.java;
      } else if (languageName.includes('cpp') || languageName.includes('c++')) {
        langMap = typeMap.cpp;
      } else if (languageName.includes('csharp') || languageName.includes('c#')) {
        langMap = typeMap.csharp;
      } else {
        langMap = typeMap.python; // fallback
      }

      return langMap[type] || type;
    },
    // Методы для работы с тестами
    addTest() {
      this.taskData.tests.push({
        input: '',
        expectedOutput: '',
        isPublic: false
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

    // Генерация сигнатуры функции
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
    }
  }
}
</script>

<style scoped>
/* Добавьте стили из предыдущего компонента редактирования */
/* Они остаются без изменений */

.loading-state {
  padding: var(--spacing-2xl);
  text-align: center;
}

.loading-icon {
  font-size: var(--font-size-hero);
  margin-bottom: var(--spacing-lg);
}

.loading-state h3 {
  margin: 0 0 var(--spacing-md) 0;
  font-size: var(--font-size-xl);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.loading-state p {
  margin: 0 0 var(--spacing-lg) 0;
  color: var(--color-on-surface-secondary);
  font-size: var(--font-size-base);
}
/* Стили из конструктора задач + дополнительные для редактирования */

.task-edit-container {
  width: 100%;
  display: block;
  min-height: 100vh;
  font-family: var(--font-family-body);
  background: var(--color-surface);
  position: relative;
}

.task-edit-wrapper {
  position: relative;
  z-index: 2;
}

.container {
  max-width: var(--content-max-width);
  margin: 0 auto;
  padding: 0 var(--spacing-lg);
}

/* Хлебные крошки */
.breadcrumbs {
  margin-bottom: var(--spacing-xl);
}

.breadcrumbs-list {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  list-style: none;
  padding: 0;
  margin: 0;
}

.breadcrumb-link {
  color: var(--color-primary);
  text-decoration: none;
  font-size: var(--font-size-sm);
  transition: color var(--animation-duration-standard) var(--animation-curve-primary);
}

.breadcrumb-link:hover {
  color: var(--color-secondary);
}

.breadcrumb-current {
  color: var(--color-on-surface-secondary);
  font-size: var(--font-size-sm);
}

.breadcrumb-separator {
  color: var(--color-border);
  font-size: var(--font-size-sm);
}

/* Заголовок страницы */
.page-header {
  margin-bottom: var(--spacing-2xl);
}

.title-section {
  text-align: center;
  margin-bottom: var(--spacing-xl);
}

.page-title {
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

.page-subtitle {
  color: var(--color-on-surface-secondary);
  font-size: var(--font-size-lg);
  margin-bottom: var(--spacing-xl);
  line-height: var(--line-height-body);
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
}

/* Статус сохранения */
.save-status {
  padding: var(--spacing-md);
  margin-top: var(--spacing-lg);
}

.status-content {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: var(--spacing-md);
  border-radius: var(--border-radius-md);
  font-weight: var(--font-weight-heading);
}

.status-content.success {
  background: color-mix(in srgb, var(--color-accent) 15%, transparent);
  color: var(--color-accent);
  border: 1px solid var(--color-accent);
}

.status-content.error {
  background: color-mix(in srgb, #EF4444 15%, transparent);
  color: #EF4444;
  border: 1px solid #EF4444;
}

.status-content.info {
  background: color-mix(in srgb, var(--color-primary) 15%, transparent);
  color: var(--color-primary);
  border: 1px solid var(--color-primary);
}

.status-icon {
  font-size: var(--font-size-base);
}

/* Основной лейаут */
.edit-layout {
  display: grid;
  grid-template-columns: 300px 1fr;
  gap: var(--spacing-xl);
  align-items: start;
  margin-bottom: var(--spacing-2xl);
}

.edit-sidebar {
  position: sticky;
  top: var(--spacing-xl);
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

/* Навигация по разделам */
.edit-nav {
  padding: var(--spacing-lg);
}

.nav-title {
  margin: 0 0 var(--spacing-lg) 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.nav-icon {
  font-size: var(--font-size-base);
}

.nav-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xs);
}

.nav-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  border-radius: var(--border-radius-md);
  cursor: pointer;
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
  border: 1px solid transparent;
  background: transparent;
  width: 100%;
  text-align: left;
  position: relative;
}

.nav-item:hover {
  background: var(--color-backplate);
  border-color: var(--color-border);
}

.nav-item.active {
  background: color-mix(in srgb, var(--color-primary) 12%, transparent);
  border-color: var(--color-primary);
  border-left: 4px solid var(--color-primary);
}

.nav-item-icon {
  font-size: var(--font-size-base);
  width: 20px;
  text-align: center;
}

.nav-item-text {
  flex: 1;
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
}

.nav-item-badge {
  background: var(--color-accent);
  color: var(--color-on-surface);
  font-size: var(--font-size-xs);
  padding: 2px 6px;
  border-radius: var(--border-radius-full);
  font-weight: var(--font-weight-heading);
  min-width: 18px;
  text-align: center;
}

/* Предпросмотр задачи */
.task-preview {
  padding: var(--spacing-lg);
}

.preview-title {
  margin: 0 0 var(--spacing-lg) 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.preview-icon {
  font-size: var(--font-size-base);
}

.preview-content {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.preview-field {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xs);
}

.preview-field label {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  font-weight: var(--font-weight-heading);
}

.preview-value {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface);
  padding: var(--spacing-xs);
  background: var(--color-backplate);
  border-radius: var(--border-radius-sm);
  border: 1px solid var(--color-border);
}

/* Действия */
.edit-actions {
  padding: var(--spacing-lg);
}

.actions-title {
  margin: 0 0 var(--spacing-lg) 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.actions-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.full-width {
  width: 100%;
}

.delete-btn {
  color: #EF4444;
  border-color: #EF4444;
}

.delete-btn:hover:not(:disabled) {
  background: color-mix(in srgb, #EF4444 15%, transparent);
  color: #EF4444;
}

/* Основное содержимое редактирования */
.edit-main {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xl);
}

.section-header {
  margin-bottom: var(--spacing-2xl);
  padding-bottom: var(--spacing-lg);
  border-bottom: 2px solid var(--color-border);
}

.section-header h2 {
  margin: 0 0 var(--spacing-md) 0;
  font-size: var(--font-size-xl);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.section-icon {
  font-size: var(--font-size-lg);
}

.section-header p {
  margin: 0;
  color: var(--color-on-surface-secondary);
  font-size: var(--font-size-base);
  line-height: var(--line-height-body);
}

/* Сообщения об ошибках */
.error-message {
  color: #EF4444;
  font-size: var(--font-size-sm);
  margin-top: var(--spacing-xs);
  font-weight: var(--font-weight-heading);
}

.input-container.error,
.tags-input.error,
.params-container.error {
  border-color: #EF4444;
}

.input-container.error input,
.input-container.error textarea,
.input-container.error select {
  background: color-mix(in srgb, #EF4444 5%, transparent);
}

/* Настройки */
.settings-grid {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.setting-item {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: var(--spacing-lg);
  padding: var(--spacing-md);
  background: var(--color-backplate);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
}

.setting-label {
  flex: 1;
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
  font-size: var(--font-size-base);
}

.setting-control {
  flex-shrink: 0;
}

.setting-description {
  flex-basis: 100%;
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  margin-top: var(--spacing-xs);
  margin-bottom: 0;
}

/* Toggle switch */
.toggle-switch {
  position: relative;
  display: inline-block;
  width: 50px;
  height: 24px;
}

.toggle-switch input {
  opacity: 0;
  width: 0;
  height: 0;
}

.toggle-slider {
  position: absolute;
  cursor: pointer;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: var(--color-border);
  transition: .4s;
  border-radius: 24px;
}

.toggle-slider:before {
  position: absolute;
  content: "";
  height: 16px;
  width: 16px;
  left: 4px;
  bottom: 4px;
  background: var(--color-surface);
  transition: .4s;
  border-radius: 50%;
}

input:checked + .toggle-slider {
  background: var(--color-primary);
}

input:checked + .toggle-slider:before {
  transform: translateX(26px);
}

/* Диалог */
.dialog-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: color-mix(in srgb, var(--color-neutral) 50%, transparent);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: var(--spacing-lg);
}

.dialog {
  padding: var(--spacing-2xl);
  max-width: 400px;
  width: 100%;
}

.dialog-title {
  margin: 0 0 var(--spacing-md) 0;
  font-size: var(--font-size-xl);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.dialog-message {
  margin: 0 0 var(--spacing-xl) 0;
  color: var(--color-on-surface-secondary);
  line-height: var(--line-height-body);
}

.dialog-actions {
  display: flex;
  gap: var(--spacing-md);
  justify-content: flex-end;
}

/* Адаптивность */
@media (max-width: 1024px) {
  .edit-layout {
    grid-template-columns: 1fr;
  }

  .edit-sidebar {
    position: static;
    order: 2;
  }
}

@media (max-width: 768px) {
  .container {
    padding: 0 var(--spacing-md);
  }

  .page-title {
    font-size: var(--font-size-xl);
    flex-direction: column;
    gap: var(--spacing-sm);
  }

  .form-grid {
    grid-template-columns: 1fr;
  }

  .dialog-actions {
    flex-direction: column;
  }

  .nav-list {
    flex-direction: row;
    overflow-x: auto;
    padding-bottom: var(--spacing-sm);
  }

  .nav-item {
    flex-shrink: 0;
    min-width: 120px;
  }
}

@media (max-width: 480px) {
  .params-header,
  .param-row {
    grid-template-columns: 1fr;
    gap: var(--spacing-xs);
  }

  .languages-grid {
    grid-template-columns: 1fr;
  }

  .test-content {
    grid-template-columns: 1fr;
  }

  .test-io {
    grid-template-columns: 1fr;
  }
}
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