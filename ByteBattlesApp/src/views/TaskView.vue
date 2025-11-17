<template>
  <div class="task-view-container">
    <app-navigation></app-navigation>

    <div class="task-view-wrapper">
      <!-- Аналогичная структура с ретро-стилями -->
      <section class="task-view-section">
        <div class="container">
          <!-- Заголовок задачи -->
          <div class="task-header retro-card">
            <div class="header-content">
              <div class="task-meta">
                <span class="task-difficulty" :class="task.difficulty">
                  {{ getDifficultyLabel(task.difficulty) }}
                </span>
                <span class="task-language">
                  <span class="lang-icon">{{ getLanguageIcon(task.language) }}</span>
                  {{ task.language }}
                </span>
              </div>
              <h1 class="task-title">{{ task.title }}</h1>
              <div class="task-actions">
                <router-link
                    v-if="canEdit"
                    :to="`/tasks/${task.id}/edit`"
                    class="btn-outline"
                >
                  <span class="btn-icon">✏️</span>
                  Редактировать
                </router-link>
                <button @click="startSolving" class="btn-primary">
                  <span class="btn-icon">🚀</span>
                  Начать решение
                </button>
              </div>
            </div>
          </div>

          <div class="task-content-layout">
            <!-- Левая колонка - описание задачи -->
            <div class="task-description-column">
              <div class="description-card retro-card">
                <h2 class="card-title">Условие задачи</h2>
                <div class="task-description" v-html="task.description"></div>

                <div class="task-requirements">
                  <h3>Требования:</h3>
                  <ul>
                    <li v-for="requirement in task.requirements" :key="requirement">
                      {{ requirement }}
                    </li>
                  </ul>
                </div>
              </div>

              <!-- Примеры -->
              <div class="examples-card retro-card" v-if="task.examples.length">
                <h2 class="card-title">Примеры</h2>
                <div
                    v-for="(example, index) in task.examples"
                    :key="index"
                    class="example-item"
                >
                  <h4>Пример {{ index + 1 }}</h4>
                  <div class="example-io">
                    <div class="input-section">
                      <strong>Вход:</strong>
                      <pre><code>{{ example.input }}</code></pre>
                    </div>
                    <div class="output-section">
                      <strong>Выход:</strong>
                      <pre><code>{{ example.output }}</code></pre>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Правая колонка - редактор кода -->
            <div class="task-editor-column">
              <div class="editor-card retro-card">
                <div class="editor-header">
                  <h2 class="card-title">Редактор кода</h2>
                  <div class="editor-actions">
                    <select v-model="selectedLanguage" class="vintage-border">
                      <option
                          v-for="lang in availableLanguages"
                          :key="lang.id"
                          :value="lang.id"
                      >
                        {{ lang.name }}
                      </option>
                    </select>
                    <button @click="resetCode" class="btn-text btn-sm">
                      <span class="btn-icon">🔄</span>
                      Сбросить
                    </button>
                  </div>
                </div>

                <div class="code-editor vintage-border">
                  <textarea
                      v-model="userCode"
                      class="code-textarea"
                      placeholder="Напишите ваше решение здесь..."
                  ></textarea>
                </div>

                <div class="editor-footer">
                  <button @click="runCode" class="btn-outline" :disabled="!userCode">
                    <span class="btn-icon">▶️</span>
                    Запустить
                  </button>
                  <button @click="submitSolution" class="btn-primary" :disabled="!userCode">
                    <span class="btn-icon">✅</span>
                    Отправить
                  </button>
                </div>
              </div>

              <!-- Результаты выполнения -->
              <div class="results-card retro-card" v-if="executionResult">
                <h2 class="card-title">Результат</h2>
                <div class="result-output vintage-border">
                  <pre>{{ executionResult.output }}</pre>
                </div>
                <div
                    class="result-status"
                    :class="{ success: executionResult.success, error: !executionResult.success }"
                >
                  {{ executionResult.success ? '✅ Успешно' : '❌ Ошибка' }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </div>

    <app-footer></app-footer>
  </div>
</template>

<script>
export default {
  name: 'TaskView',
  props: ['taskId'],
  data() {
    return {
      task: {},
      userCode: '',
      selectedLanguage: 'python',
      executionResult: null
    }
  },
  computed: {
    canEdit() {
      // Логика проверки прав на редактирование
      return this.task.isAuthor || this.user.isTeacher
    }
  },
  async mounted() {
    await this.loadTask()
    this.loadStarterCode()
  },
  methods: {
    async loadTask() {
      // Загрузка данных задачи
    },
    loadStarterCode() {
      // Загрузка стартового кода
    },
    async runCode() {
      // Запуск кода
    },
    async submitSolution() {
      // Отправка решения
    },
    resetCode() {
      this.userCode = this.task.starterCode[this.selectedLanguage]
    },
    startSolving() {
      // Начать решение
    }
  }
}
</script>