<template>
  <div class="task-list-container">
    <app-navigation></app-navigation>

    <div class="task-list-wrapper">
      <DangerousHTML
          html="<style>
  .task-list-container {
    min-height: 100vh;
    background: var(--color-surface);
    padding: var(--spacing-2xl) 0;
  }

  .task-list-container::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-image:
      radial-gradient(circle at 20% 80%, color-mix(in srgb, var(--color-secondary) 6%, transparent) 0%, transparent 50%),
      repeating-linear-gradient(
        45deg,
        transparent,
        transparent 2px,
        color-mix(in srgb, var(--color-border) 3%, transparent) 2px,
        color-mix(in srgb, var(--color-border) 3%, transparent) 4px
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

  @keyframes fadeIn {
    from {
      opacity: 0;
      transform: translateY(10px);
    }
    to {
      opacity: 1;
      transform: translateY(0);
    }
  }

  .task-card {
    animation: fadeIn 0.4s var(--animation-curve-primary);
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

      <section class="task-list-section" role="main" aria-label="Список задач">
        <div class="container">
          <!-- Заголовок и фильтры -->
          <div class="page-header">
            <div class="title-section">
              <h1 class="page-title">

                Библиотека задач
              </h1>
              <p class="page-subtitle">
                Изучайте программирование через решение практических задач
              </p>
            </div>
          </div>

          <!-- Фильтры -->
          <div class="tasks-filters retro-card">
            <div class="filters-header">
              <h3 class="filters-title">

                Фильтры задач
              </h3>
              <div>
                <button
                    @click="resetFilters"
                    class="btn-text btn-sm reset-filters-btn"
                    :disabled="!hasActiveFilters"
                >

                  Сбросить
                </button>
                <div class="view-controls">
                  <button
                      @click="viewMode = 'grid'"
                      :class="['view-btn', { 'active': viewMode === 'grid' }]"
                      title="Сетка"
                  >
                    <span class="btn-icon">⏹️</span>
                  </button>
                  <button
                      @click="viewMode = 'list'"
                      :class="['view-btn', { 'active': viewMode === 'list' }]"
                      title="Список"
                  >
                    <span class="btn-icon">📋</span>
                  </button>
                </div>
              </div>
            </div>

            <div class="filters-grid">
              <div class="filter-group">
                <label class="filter-label">Сложность:</label>
                <div class="select-wrapper vintage-border">
                  <select v-model="difficultyFilter" class="filter-select">
                    <option value="">Все уровни</option>
                    <option value="easy">Легкие</option>
                    <option value="medium">Средние</option>
                    <option value="hard">Сложные</option>
                  </select>
                  <span class="select-arrow">▼</span>
                </div>
              </div>

              <div class="filter-group">
                <label class="filter-label">Язык:</label>
                <div class="select-wrapper vintage-border">
                  <select v-model="languageFilter" class="filter-select">
                    <option value="">Все языки</option>
                    <option v-for="lang in availableLanguages" :key="lang.id" :value="lang.id">
                      {{ lang.name }}
                    </option>
                  </select>
                  <span class="select-arrow">▼</span>
                </div>
              </div>

              <div class="filter-group">
                <label class="filter-label">Категория:</label>
                <div class="select-wrapper vintage-border">
                  <select v-model="categoryFilter" class="filter-select">
                    <option value="">Все категории</option>
                    <option value="algorithms">Алгоритмы</option>
                    <option value="data-structures">Структуры данных</option>
                    <option value="oop">ООП</option>
                    <option value="web">Веб-разработка</option>
                    <option value="databases">Базы данных</option>
                  </select>
                  <span class="select-arrow">▼</span>
                </div>
              </div>

              <div class="search-group">
                <label class="filter-label">Поиск:</label>
                <div class="search-input-wrapper vintage-border">

                  <input
                      type="text"
                      v-model="searchTerm"
                      placeholder="Название задачи..."
                      class="search-input"
                  >
                  <button
                      v-if="searchTerm"
                      @click="searchTerm = ''"
                      class="clear-search-btn"
                      type="button"
                  >
                    <span class="clear-icon">×</span>
                  </button>

                </div>

              </div>

            </div>

            <!-- Активные фильтры -->
            <div class="active-filters" v-if="hasActiveFilters">
              <div class="active-filters-label">Активные фильтры:</div>
              <div class="active-filters-tags">
                <span
                    v-if="difficultyFilter"
                    class="filter-tag"
                    @click="difficultyFilter = ''"
                >
                  Сложность: {{ getDifficultyLabel(difficultyFilter) }}
                  <span class="tag-remove">×</span>
                </span>
                <span
                    v-if="languageFilter"
                    class="filter-tag"
                    @click="languageFilter = ''"
                >
                  Язык: {{ getLanguageName(languageFilter) }}
                  <span class="tag-remove">×</span>
                </span>
                <span
                    v-if="categoryFilter"
                    class="filter-tag"
                    @click="categoryFilter = ''"
                >
                  Категория: {{ getCategoryLabel(categoryFilter) }}
                  <span class="tag-remove">×</span>
                </span>
                <span
                    v-if="searchTerm"
                    class="filter-tag"
                    @click="searchTerm = ''"
                >
                  Поиск: "{{ searchTerm }}"
                  <span class="tag-remove">×</span>
                </span>
              </div>
            </div>
          </div>


          <!-- Список задач -->
          <div class="tasks-container">
            <div :class="['tasks-grid', viewMode]">
              <div
                  v-for="task in paginatedTasks"
                  :key="task.id"
                  class="task-card retro-card"
                  :class="{ 'solved': task.isSolved, 'featured': task.isFeatured }"
              >
                <div class="task-header">
                  <div class="task-meta">
                    <span class="task-difficulty" :class="task.difficulty">
                      {{ getDifficultyLabel(task.difficulty) }}
                    </span>
                    <span class="task-language">
  <span class="lang-icon">{{ getLanguageIcon(task.language) }}</span>
  {{ task.language }}
</span>
                    <span class="task-category" v-if="task.category">
                      {{ getCategoryLabel(task.category) }}
                    </span>
                  </div>
                  <div class="task-actions">
                    <span class="task-status" v-if="task.isSolved">
                      Решена
                    </span>
                    <button
                        v-if="task.isAuthor || userRole === ''"
                        @click="editTask(task.id)"
                        class="btn-text btn-sm"
                        title="Редактировать"
                    >
                      Редактировать
                    </button>
                  </div>
                </div>

                <h3 class="task-title">{{ task.title }}</h3>
                <p class="task-description">{{ truncateDescription(task.description) }}</p>

                <div class="task-stats">
                  <div class="stat">
                    <span class="stat-icon">⭐</span>
                    <span class="stat-value">{{ task.completedCount }}</span>
                    <span class="stat-label">решали</span>
                  </div>
                  <div class="stat">
                    <span class="stat-icon">✅</span>
                    <span class="stat-value">{{ task.SuccessfulAttempts }}</span>
                    <span class="stat-label">Успешные попытки</span>
                  </div>
                  <div class="stat">
                    <span class="stat-icon">⏱️</span>
                    <span class="stat-value">{{ task.AverageExecutionTime }}м</span>
                    <span class="stat-label">Среднее время выполнения</span>
                  </div>
                </div>

                <div class="task-tags">
                  <span
                      v-for="tag in task.tags.slice(0, 3)"
                      :key="tag"
                      class="tag"
                  >
                    {{ tag }}
                  </span>
                  <span
                      v-if="task.tags.length > 3"
                      class="tag-more"
                      :title="`Еще теги: ${task.tags.slice(3).join(', ')}`"
                  >
                    +{{ task.tags.length - 3 }}
                  </span>
                </div>

                <div class="task-footer">
                  <div class="task-author" v-if="task.author">
                    <span class="author-avatar">
                      {{ task.author.name.charAt(0).toUpperCase() }}
                    </span>
                    <span class="author-name">{{ task.author.name }}</span>
                  </div>
                  <div class="task-actions-main">
                    <router-link
                        :to="`/tasks/${task.id}`"
                        class="btn-outline btn-sm"
                    >

                      {{ task.isSolved ? 'Просмотр' : 'Решать' }}
                    </router-link>
                    <button
                        v-if="task.isSolved"
                        @click="viewSolution(task.id)"
                        class="btn-text btn-sm"
                        title="Посмотреть решение"
                    >

                    </button>
                  </div>
                </div>

                <!-- Бейдж избранной задачи -->
                <div class="featured-badge" v-if="task.isFeatured">

                  Избранная
                </div>
              </div>
            </div>

            <!-- Пагинация -->
            <div class="pagination-controls retro-card" v-if="totalPages > 1">
              <div class="pagination-info">
                Показано {{ startItem }}-{{ endItem }} из {{ filteredTasks.length }} задач
              </div>
              <div class="pagination-buttons">
                <button
                    @click="prevPage"
                    :disabled="currentPage === 1"
                    class="btn-outline btn-sm"
                >
                  <span class="btn-icon">←</span>
                  Назад
                </button>

                <div class="page-numbers">
                  <button
                      v-for="page in visiblePages"
                      :key="page"
                      @click="goToPage(page)"
                      :class="['page-btn', { active: currentPage === page }]"
                  >
                    {{ page }}
                  </button>
                </div>

                <button
                    @click="nextPage"
                    :disabled="currentPage === totalPages"
                    class="btn-outline btn-sm"
                >
                  Вперед
                  <span class="btn-icon">→</span>
                </button>
              </div>
            </div>

            <!-- Пустое состояние -->
            <div class="empty-state retro-card" v-if="filteredTasks.length === 0">
              <div class="empty-icon">🔍</div>
              <h3>Задачи не найдены</h3>
              <p>Попробуйте изменить параметры поиска или создать новую задачу</p>
              <div class="empty-actions">
                <button @click="resetFilters" class="btn-primary">
                  <span class="btn-icon">🔄</span>
                  Сбросить фильтры
                </button>
                <router-link to="/task-template-builder" class="btn-outline" v-if="userRole === 'teacher'">
                  <span class="btn-icon">➕</span>
                  Создать задачу
                </router-link>
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
import DangerousHTML from 'dangerous-html/vue'
import AppNavigation from '../components/navigation'
import AppFooter from '../components/footer'
import {languageAPI, taskAPI} from '../api/task.js'

export default {
  name: 'TaskList',
  components: {
    AppNavigation,
    DangerousHTML,
    AppFooter,
  },
  data() {
    return {
      viewMode: 'grid',
      difficultyFilter: '',
      languageFilter: '',
      categoryFilter: '',
      searchTerm: '',
      currentPage: 1,
      pageSize: 12,
      isAuthor: false,
      userRole: 'teacher', // или 'teacher'

      tasks: [],

      availableLanguages: []
    }
  },
  async mounted() {
    await this.loadLanguages()
    await this.getAllTasks()
  },
  computed: {
    filteredTasks() {
      return this.tasks.filter(task => {
        const matchesDifficulty = !this.difficultyFilter || task.difficulty === this.difficultyFilter
        const matchesLanguage = !this.languageFilter || task.languageId === this.languageFilter
        const matchesCategory = !this.categoryFilter || task.category === this.categoryFilter
        const matchesSearch = !this.searchTerm ||
            task.title.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            (task.description && task.description.toLowerCase().includes(this.searchTerm.toLowerCase())) ||
            (task.tags && task.tags.some(tag => tag.toLowerCase().includes(this.searchTerm.toLowerCase())))

        return matchesDifficulty && matchesLanguage && matchesCategory && matchesSearch
      })
    },

    paginatedTasks() {
      const start = (this.currentPage - 1) * this.pageSize
      return this.filteredTasks.slice(start, start + this.pageSize)
    },

    totalPages() {
      return Math.ceil(this.filteredTasks.length / this.pageSize)
    },

    visiblePages() {
      const pages = []
      const start = Math.max(1, this.currentPage - 2)
      const end = Math.min(this.totalPages, start + 4)

      for (let i = start; i <= end; i++) {
        pages.push(i)
      }
      return pages
    },

    hasActiveFilters() {
      return this.difficultyFilter || this.languageFilter || this.categoryFilter || this.searchTerm
    },

    totalTasks() {
      return this.tasks.length
    },

    completedTasks() {
      return this.tasks.filter(task => task.isSolved).length
    },

    averageRating() {
      const total = this.tasks.reduce((sum, task) => sum + task.rating, 0)
      return (total / this.tasks.length).toFixed(1)
    },

    activeFilteredTasks() {
      return this.filteredTasks.length
    },

    startItem() {
      return (this.currentPage - 1) * this.pageSize + 1
    },

    endItem() {
      return Math.min(this.currentPage * this.pageSize, this.filteredTasks.length)
    }
  },
  watch: {
    difficultyFilter() {
      this.currentPage = 1
    },
    languageFilter() {
      this.currentPage = 1
    },
    categoryFilter() {
      this.currentPage = 1
    },
    searchTerm() {
      this.currentPage = 1
    }
  },
  methods: {

    async getAllTasks() {
      this.isLoading = true
      this.error = null
      try {
        const tasks = await taskAPI.getAll()
        console.log('Полученные задачи из API:', tasks) // Для отладки

        this.tasks = tasks.map(task => ({
          id: task.id,
          title: task.title,
          description: task.description,
          difficulty: task.difficulty,
          // Исправляем получение языка
          language: task.taskLanguages && task.taskLanguages.length > 0
              ? task.taskLanguages[0].title
              : 'Unknown',
          // Язык для фильтрации (ID)
          languageId: task.taskLanguages && task.taskLanguages.length > 0
              ? task.taskLanguages[0].idLanguage
              : '',
          author: {
            name: task.author || 'Unknown Author'
          },
          isAuthor: task.author === JSON.parse(localStorage.getItem("user")).firstName,
          functionName: task.functionName,
          patternMain: task.patternMain,
          patternFunction: task.patternFunction,
          completedCount: task.completedCount || 0,
          SuccessfulAttempts: task.successfulAttempts || 0,
          AverageExecutionTime: task.averageExecutionTime ? Math.round(task.averageExecutionTime) : 0,

          category: task.category || 'algorithms',
          tags: task.tags || [],
          isSolved: task.isSolved || false,
          isFeatured: task.isFeatured || false,
          rating: task.rating || 4.5
        }))
        console.log(JSON.parse(localStorage.getItem("user")).firstName)

        console.log('Преобразованные задачи:', this.tasks) // Для отладки

      } catch (error) {
        console.error('Ошибка при загрузке задач:', error)
        this.error = 'Не удалось загрузить список задач'
      } finally {
        this.isLoading = false
      }
    },
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
    resetFilters() {
      this.difficultyFilter = ''
      this.languageFilter = ''
      this.categoryFilter = ''
      this.searchTerm = ''
      this.currentPage = 1
    },

    editTask(taskId) {
      this.$router.push(`/tasks/${taskId}/edit`)
    },

    viewSolution(taskId) {
      console.log('Просмотр решения задачи:', taskId)
      // Логика просмотра решения
    },

    getDifficultyLabel(difficulty) {
      const labels = {
        Easy: 'Легкая',
        Medium: 'Средняя',
        Hard: 'Сложная'
      }
      return labels[difficulty] || difficulty
    },

    getLanguageName(langId) {
      const lang = this.availableLanguages.find(l => l.id === langId)
      return lang ? lang.name : langId
    },

    getCategoryLabel(category) {
      const labels = {
        algorithms: 'Алгоритмы',
        'data-structures': 'Структуры данных',
        oop: 'ООП',
        web: 'Веб-разработка',
        databases: 'Базы данных'
      }
      return labels[category] || category
    },

    getLanguageIcon(language) {
      const icons = {
        Python: '🐍',
        Java: '☕',
        JavaScript: '📜',
        'C++': '⚡',
        SQL: '🗄️',
        'C#': '🎵'
      }
      return icons[language] || '💻'
    },

    truncateDescription(description) {
      const maxLength = this.viewMode === 'grid' ? 120 : 200
      return description.length > maxLength
          ? description.substring(0, maxLength) + '...'
          : description
    },

    prevPage() {
      if (this.currentPage > 1) {
        this.currentPage--
      }
    },

    nextPage() {
      if (this.currentPage < this.totalPages) {
        this.currentPage++
      }
    },

    goToPage(page) {
      this.currentPage = page
    }
  }
}
</script>

<style scoped>
.task-list-container {
  width: 100%;
  display: block;
  min-height: 100vh;
  font-family: var(--font-family-body);
  background: var(--color-surface);
  position: relative;
}

.task-list-wrapper {
  position: relative;
  z-index: 2;
}

.container {
  max-width: var(--content-max-width);
  margin: 0 auto;
  padding: 0 var(--spacing-lg);
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

/* Быстрые действия */
.quick-actions {
  padding: var(--spacing-lg);
}

.actions-grid {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.view-controls {
  display: flex;
  gap: var(--spacing-xs);
  background: var(--color-backplate);
  padding: var(--spacing-xs);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
}

.view-btn {
  padding: var(--spacing-sm) var(--spacing-md);
  border: none;
  background: transparent;
  border-radius: var(--border-radius-sm);
  cursor: pointer;
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
}

.view-btn.active {
  background: var(--color-primary);
  color: var(--color-on-primary);
}

/* Фильтры */
.tasks-filters {
  padding: var(--spacing-lg);
  margin-bottom: var(--spacing-lg);
}

.filters-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-lg);
}

.filters-title {
  margin: 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.filters-icon {
  font-size: var(--font-size-base);
}

.reset-filters-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.filters-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--spacing-lg);
  align-items: end;
}

.filter-group,
.search-group {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.filter-label {
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
  font-size: var(--font-size-sm);
  margin-bottom: 0;
}

.select-wrapper {
  position: relative;
  display: flex;
  align-items: center;
  padding: 0;
  background: var(--color-surface);
}

.filter-select {
  width: 100%;
  padding: var(--spacing-md);
  border: none;
  background: transparent;
  font-size: var(--font-size-base);
  color: var(--color-on-surface);
  appearance: none;
  cursor: pointer;
  padding-right: var(--spacing-xl);
}

.filter-select:focus {
  outline: none;
}

.select-arrow {
  position: absolute;
  right: var(--spacing-md);
  pointer-events: none;
  color: var(--color-on-surface-secondary);
  font-size: var(--font-size-sm);
  transition: transform var(--animation-duration-standard) var(--animation-curve-primary);
}

.select-wrapper:focus-within .select-arrow {
  transform: rotate(180deg);
}

/* Поиск */
.search-input-wrapper {
  display: flex;
  align-items: center;
  padding: var(--spacing-sm);
  gap: var(--spacing-sm);
}

.search-icon {
  color: var(--color-on-surface-secondary);
  font-size: var(--font-size-base);
  flex-shrink: 0;
}

.search-input {
  flex: 1;
  border: none;
  background: transparent;
  font-size: var(--font-size-base);
  color: var(--color-on-surface);
  padding: 0;
}

.search-input::placeholder {
  color: var(--color-on-surface-secondary);
}

.search-input:focus {
  outline: none;
}

.clear-search-btn {
  background: none;
  border: none;
  color: var(--color-on-surface-secondary);
  cursor: pointer;
  padding: var(--spacing-xs);
  border-radius: var(--border-radius-sm);
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
  display: flex;
  align-items: center;
  justify-content: center;
  width: 20px;
  height: 20px;
}

.clear-search-btn:hover {
  background: var(--color-backplate);
  color: var(--color-on-surface);
}

.clear-icon {
  font-size: var(--font-size-lg);
  line-height: 1;
}

/* Активные фильтры */
.active-filters {
  margin-top: var(--spacing-lg);
  padding-top: var(--spacing-md);
  border-top: 1px solid var(--color-border);
}

.active-filters-label {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  margin-bottom: var(--spacing-sm);
  font-weight: var(--font-weight-heading);
}

.active-filters-tags {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-sm);
}

.filter-tag {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-xs);
  background: color-mix(in srgb, var(--color-primary) 15%, transparent);
  color: var(--color-primary);
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius-full);
  font-size: var(--font-size-sm);
  border: 1px solid var(--color-primary);
  cursor: pointer;
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
}

.filter-tag:hover {
  background: color-mix(in srgb, var(--color-primary) 25%, transparent);
  transform: translateY(-1px);
}

.tag-remove {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-heading);
  margin-left: var(--spacing-xs);
}

/* Статистика */
.tasks-stats {
  padding: var(--spacing-lg);
  margin-bottom: var(--spacing-lg);
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--spacing-lg);
}

.stat-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  background: var(--color-backplate);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
  text-align: left;
}

.stat-icon {
  font-size: var(--font-size-xl);
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-surface);
  border-radius: var(--border-radius-md);
}

.stat-data {
  display: flex;
  flex-direction: column;
}

.stat-value {
  font-size: var(--font-size-xl);
  font-weight: var(--font-weight-heading);
  color: var(--color-primary);
  margin-bottom: var(--spacing-xs);
  font-family: var(--font-family-heading);
}

.stat-label {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
}

/* Контейнер задач */
.tasks-container {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.tasks-grid {
  display: grid;
  gap: var(--spacing-lg);
}

.tasks-grid.grid {
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
}

.tasks-grid.list {
  grid-template-columns: 1fr;
}

/* Карточка задачи */
.task-card {
  padding: var(--spacing-lg);
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
  position: relative;
}

.task-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-level-2);
}

.task-card.solved {
  border-left: 4px solid var(--color-accent);
}

.task-card.featured {
  border: 2px solid color-mix(in srgb, var(--color-accent) 30%, transparent);
}

.task-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--spacing-md);
}

.task-meta {
  display: flex;
  gap: var(--spacing-md);
  font-size: var(--font-size-sm);
  flex-wrap: wrap;
}

.task-difficulty {
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius-full);
  font-weight: var(--font-weight-heading);
  font-size: var(--font-size-xs);
}

.task-difficulty.easy {
  background: color-mix(in srgb, #10B981 15%, transparent);
  color: #10B981;
  border: 1px solid #10B981;
}

.task-difficulty.medium {
  background: color-mix(in srgb, #3B82F6 15%, transparent);
  color: #3B82F6;
  border: 1px solid #3B82F6;
}

.task-difficulty.hard {
  background: color-mix(in srgb, #EF4444 15%, transparent);
  color: #EF4444;
  border: 1px solid #EF4444;
}

.task-language,
.task-category {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
  color: var(--color-on-surface-secondary);
  background: var(--color-backplate);
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius-full);
  font-size: var(--font-size-xs);
}

.lang-icon {
  font-size: var(--font-size-base);
}

.task-actions {
  display: flex;
  gap: var(--spacing-sm);
  align-items: center;
}

.task-status {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
  font-size: var(--font-size-sm);
  color: var(--color-accent);
  font-weight: var(--font-weight-heading);
}

.status-icon {
  font-size: var(--font-size-base);
}

.task-title {
  margin: 0 0 var(--spacing-sm) 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
  line-height: var(--line-height-heading);
}

.task-description {
  margin: 0 0 var(--spacing-lg) 0;
  color: var(--color-on-surface-secondary);
  font-size: var(--font-size-sm);
  line-height: var(--line-height-body);
}

/* Статистика задачи */
.task-stats {
  display: flex;
  gap: var(--spacing-lg);
  margin-bottom: var(--spacing-lg);
  padding: var(--spacing-md);
  background: var(--color-backplate);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
}

.stat {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--spacing-xs);
  font-size: var(--font-size-sm);
  flex: 1;
}

.stat-icon {
  font-size: var(--font-size-base);
  width: auto;
  height: auto;
  background: none;
}

.stat-value {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
}

.stat-label {
  font-size: var(--font-size-xs);
  color: var(--color-on-surface-secondary);
  text-transform: lowercase;
}

/* Теги задачи */
.task-tags {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-xs);
  margin-bottom: var(--spacing-lg);
}

.tag {
  background: var(--color-backplate);
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius-full);
  font-size: var(--font-size-xs);
  color: var(--color-on-surface-secondary);
  border: 1px solid var(--color-border);
}

.tag-more {
  background: var(--color-primary);
  color: var(--color-on-primary);
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius-full);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-heading);
  cursor: help;
}

/* Футер задачи */
.task-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.task-author {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
}

.author-avatar {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  background: var(--color-primary);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--color-on-primary);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-heading);
}

.author-name {
  font-weight: var(--font-weight-heading);
}

.task-actions-main {
  display: flex;
  gap: var(--spacing-sm);
  align-items: center;
}

/* Бейдж избранной задачи */
.featured-badge {
  position: absolute;
  top: -8px;
  right: -8px;
  background: linear-gradient(135deg, var(--color-accent), var(--color-primary));
  color: var(--color-on-surface);
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius-full);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-heading);
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
  box-shadow: var(--shadow-level-1);
}

.badge-icon {
  font-size: var(--font-size-sm);
}

/* Пагинация */
.pagination-controls {
  padding: var(--spacing-lg);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.pagination-info {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
}

.pagination-buttons {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
}

.page-numbers {
  display: flex;
  gap: var(--spacing-xs);
}

.page-btn {
  width: 40px;
  height: 40px;
  border: 1px solid var(--color-border);
  background: var(--color-surface);
  border-radius: var(--border-radius-md);
  cursor: pointer;
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
  font-size: var(--font-size-base);
  color: var(--color-on-surface);
}

.page-btn:hover {
  border-color: var(--color-primary);
  color: var(--color-primary);
}

.page-btn.active {
  background: var(--color-primary);
  color: var(--color-on-primary);
  border-color: var(--color-primary);
}

/* Пустое состояние */
.empty-state {
  padding: var(--spacing-2xl);
  text-align: center;
}

.empty-icon {
  font-size: var(--font-size-hero);
  margin-bottom: var(--spacing-lg);
}

.empty-state h3 {
  margin: 0 0 var(--spacing-md) 0;
  font-size: var(--font-size-xl);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.empty-state p {
  margin: 0 0 var(--spacing-lg) 0;
  color: var(--color-on-surface-secondary);
  font-size: var(--font-size-base);
}

.empty-actions {
  display: flex;
  gap: var(--spacing-md);
  justify-content: center;
  flex-wrap: wrap;
}

/* Кнопки (используем стили из конструктора задач) */
.btn-primary,
.btn-outline,
.btn-text,
.btn-sm {
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

.btn-primary {
  background: var(--color-primary);
  border-color: var(--color-primary);
  color: var(--color-on-primary);
}

.btn-primary:hover:not(:disabled) {
  background: color-mix(in srgb, var(--color-primary) 85%, black);
  border-color: color-mix(in srgb, var(--color-primary) 85%, black);
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

.btn-text {
  background: transparent;
  border-color: transparent;
  color: var(--color-on-surface);
}

.btn-text:hover:not(:disabled) {
  background: var(--color-backplate);
  color: var(--color-primary);
}

.btn-sm {
  padding: var(--spacing-sm) var(--spacing-md);
  font-size: var(--font-size-sm);
}

.btn-icon {
  font-size: var(--font-size-base);
}

/* Адаптивность */
@media (max-width: 1200px) {
  .tasks-grid.grid {
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  }
}

@media (max-width: 1024px) {
  .filters-grid {
    grid-template-columns: 1fr 1fr;
  }

  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
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

  .filters-grid {
    grid-template-columns: 1fr;
  }

  .filters-header {
    flex-direction: column;
    align-items: flex-start;
    gap: var(--spacing-md);
  }

  .reset-filters-btn {
    align-self: stretch;
    text-align: center;
  }

  .tasks-grid.grid {
    grid-template-columns: 1fr;
  }

  .actions-grid {
    flex-direction: column;
    gap: var(--spacing-md);
    align-items: stretch;
  }

  .view-controls {
    align-self: center;
  }

  .task-header {
    flex-direction: column;
    gap: var(--spacing-md);
    align-items: flex-start;
  }

  .task-actions {
    align-self: stretch;
    justify-content: space-between;
  }

  .task-footer {
    flex-direction: column;
    gap: var(--spacing-md);
    align-items: flex-start;
  }

  .task-actions-main {
    align-self: stretch;
    justify-content: space-between;
  }

  .pagination-controls {
    flex-direction: column;
    gap: var(--spacing-md);
    text-align: center;
  }

  .page-numbers {
    display: none;
  }

  .empty-actions {
    flex-direction: column;
    align-items: center;
  }
}

@media (max-width: 480px) {
  .task-meta {
    flex-direction: column;
    gap: var(--spacing-sm);
    align-items: flex-start;
  }

  .task-stats {
    flex-direction: column;
    gap: var(--spacing-md);
  }

  .stat {
    flex-direction: row;
    justify-content: space-between;
  }

  .stats-grid {
    grid-template-columns: 1fr;
  }
}
</style>