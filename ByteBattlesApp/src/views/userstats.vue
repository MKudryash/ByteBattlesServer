<template>
  <div class="users-stats-container">
    <app-navigation></app-navigation>

    <div class="users-stats-wrapper">
      <DangerousHTML
          html="<style>
  .users-stats-container {
    min-height: 100vh;
    background: var(--color-surface);
    padding: var(--spacing-2xl) 0;
  }

  .users-stats-container::before {
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

  .user-card {
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

      <section class="users-stats-section" role="main" aria-label="Пользователи и статистика">
        <div class="container">
          <!-- Заголовок и фильтры -->
          <div class="quick-filters retro-card">
            <div class="filters-header">
              <h3 class="filters-title">
                Фильтры и поиск
              </h3>
              <button
                  @click="resetFilters"
                  class="btn-text btn-sm reset-filters-btn"
                  :disabled="!hasActiveFilters"
              >
                Сбросить
              </button>
            </div>

            <div class="filters-grid">
              <div class="filter-group">
                <label class="filter-label">Сортировка:</label>
                <div class="select-wrapper vintage-border">
                  <select v-model="sortBy" class="filter-select">
                    <option value="rating">По рейтингу</option>
                    <option value="tasks">По задачам</option>
                    <option value="recent">По активности</option>
                    <option value="name">По имени</option>
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
                <label class="filter-label">Уровень:</label>
                <div class="select-wrapper vintage-border">
                  <select v-model="levelFilter" class="filter-select">
                    <option value="">Все уровни</option>
                    <option value="beginner">Начинающий</option>
                    <option value="intermediate">Средний</option>
                    <option value="advanced">Продвинутый</option>
                    <option value="expert">Эксперт</option>
                  </select>
                  <span class="select-arrow">▼</span>
                </div>
              </div>

              <div class="search-group">
                <label class="filter-label">Поиск:</label>
                <div class="search-input-wrapper vintage-border">
                  <span class="search-icon">🔍</span>
                  <input
                      type="text"
                      v-model="searchTerm"
                      placeholder="Введите имя пользователя..."
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

            <!-- Индикаторы активных фильтров -->
            <div class="active-filters" v-if="hasActiveFilters">
              <div class="active-filters-label">Активные фильтры:</div>
              <div class="active-filters-tags">
      <span
          v-if="languageFilter"
          class="filter-tag"
          @click="languageFilter = ''"
      >
        Язык: {{ getLanguageName(languageFilter) }}
        <span class="tag-remove">×</span>
      </span>
                <span
                    v-if="levelFilter"
                    class="filter-tag"
                    @click="levelFilter = ''"
                >
        Уровень: {{ getLevelLabel(levelFilter) }}
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

          <div class="content-layout">
            <!-- Боковая панель с лидербордом -->
            <aside class="sidebar-panel" role="complementary" aria-label="Топ пользователей">
              <!-- Лидерборд -->
              <div class="leaderboard-section retro-card">
                <div class="section-header">
                  <h2>
                    <span class="header-icon">🏆</span>
                    Топ рейтинга
                  </h2>
                  <button @click="refreshLeaderboard" class="btn-text btn-sm">
                  </button>
                </div>

                <div class="leaderboard-list">
                  <div
                      v-for="(user, index) in leaderboard"
                      :key="user.id"
                      :class="['leaderboard-item', `rank-${index + 1}`]"
                  >
                    <div class="rank-badge">
                      <span class="rank-number">{{ index + 1 }}</span>
                      <div class="rank-crown" v-if="index < 3">
                        {{ ['👑', '🥈', '🥉'][index] }}
                      </div>
                    </div>
                    <div class="user-avatar">
                      <img :src="user.avatar" :alt="user.name" v-if="user.avatar">
                      <div class="avatar-placeholder" v-else>
                        {{ user.name.charAt(0).toUpperCase() }}
                      </div>
                    </div>
                    <div class="user-info">
                      <h3 class="user-name">{{ user.name }}</h3>
                      <div class="user-stats">
                        <span class="rating">⭐ {{ user.rating }}</span>
                        <span class="tasks">✅ {{ user.completedTasks }}</span>
                      </div>
                    </div>
                    <div class="progress-ring">
                      <svg width="40" height="40" viewBox="0 0 40 40">
                        <circle
                            cx="20"
                            cy="20"
                            r="18"
                            stroke="var(--color-border)"
                            stroke-width="3"
                            fill="none"
                        ></circle>
                        <circle
                            cx="20"
                            cy="20"
                            r="18"
                            :stroke="getRankColor(index)"
                            stroke-width="3"
                            fill="none"
                            :stroke-dasharray="113"
                            :stroke-dashoffset="113 - (user.progress * 113 / 100)"
                            stroke-linecap="round"
                            transform="rotate(-90 20 20)"
                        ></circle>
                      </svg>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Глобальная статистика -->
              <div class="global-stats retro-card">
                <h2>
                  Статистика сообщества
                </h2>
                <div class="stats-grid">
                  <div class="stat-card">
                    <div class="stat-data">
                      <span class="stat-value">{{ communityStats.totalUsers }}</span>
                      <span class="stat-label">пользователей</span>
                    </div>
                  </div>
                  <div class="stat-card">
                    <div class="stat-data">
                      <span class="stat-value">{{ communityStats.totalTasks }}</span>
                      <span class="stat-label">решенных задач</span>
                    </div>
                  </div>
                  <div class="stat-card">
                    <div class="stat-data">
                      <span class="stat-value">{{ communityStats.activeToday }}</span>
                      <span class="stat-label">активных сегодня</span>
                    </div>
                  </div>
                  <div class="stat-card">
                    <div class="stat-data">
                      <span class="stat-value">{{ communityStats.countries }}</span>
                      <span class="stat-label">стран</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Языки программирования -->
              <div class="languages-stats retro-card">
                <h2>
                  <span class="header-icon">💻</span>
                  Популярные языки
                </h2>
                <div class="languages-list">
                  <div
                      v-for="lang in popularLanguages"
                      :key="lang.name"
                      class="language-item"
                  >
                    <div class="lang-info">
                      <span class="lang-icon">{{ lang.icon }}</span>
                      <span class="lang-name">{{ lang.name }}</span>
                    </div>
                    <div class="lang-stats">
                      <span class="lang-percentage">{{ lang.percentage }}%</span>
                      <div class="progress-bar">
                        <div
                            class="progress-fill"
                            :style="{ width: lang.percentage + '%' }"
                            :class="lang.color"
                        ></div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </aside>

            <!-- Основное содержимое -->
            <main class="main-content" role="region" aria-label="Список пользователей">
              <!-- Заголовок и управление -->
              <div class="content-header retro-card">
                <div class="header-left">
                  <h2>Все пользователи</h2>
                  <span class="users-count">Найдено {{ filteredUsers.length }} пользователей</span>
                </div>
                <div class="header-right">
                  <div class="view-controls">
                    <button
                        @click="viewMode = 'grid'"
                        :class="['view-btn', { 'active': viewMode === 'grid' }]"
                    >
                      <span class="btn-icon">⏹️</span>
                    </button>
                    <button
                        @click="viewMode = 'list'"
                        :class="['view-btn', { 'active': viewMode === 'list' }]"
                    >
                      <span class="btn-icon">📋</span>
                    </button>
                  </div>
                </div>
              </div>

              <!-- Список пользователей -->
              <div :class="['users-container', viewMode]">
                <div
                    v-for="user in paginatedUsers"
                    :key="user.id"
                    class="user-card retro-card"
                >
                  <!-- Аватар и основная информация -->
                  <div class="user-header">
                    <div class="user-avatar-large">
                      <img :src="user.avatar" :alt="user.name" v-if="user.avatar">
                      <div class="avatar-placeholder-large" v-else>
                        {{ user.name.charAt(0).toUpperCase() }}
                      </div>
                      <div class="online-indicator" :class="{ online: user.isOnline }"></div>
                    </div>

                    <div class="user-main-info">
                      <h3 class="user-name">{{ user.name }}</h3>
                      <p class="user-bio" v-if="user.bio">{{ user.bio }}</p>
                      <div class="user-meta">
                        <span class="user-country" v-if="user.country">
                          <span class="meta-icon">🌍</span>
                          {{ user.country }}
                        </span>
                        <span class="user-level" :class="user.level">
                          <span class="meta-icon">{{ getLevelIcon(user.level) }}</span>
                          {{ getLevelLabel(user.level) }}
                        </span>
                      </div>
                    </div>

                    <div class="user-actions">
                      <button @click="viewProfile(user.id)" class="btn-outline btn-sm">
                        Профиль
                      </button>
                    </div>
                  </div>

                  <!-- Статистика -->
                  <div class="user-stats-grid">
                    <div class="stat-item">
                      <span class="stat-value">{{ user.rating }}</span>
                      <span class="stat-label"> Рейтинг</span>
                    </div>
                    <div class="stat-item">
                      <span class="stat-value">{{ user.completedTasks }}</span>
                      <span class="stat-label">Задач</span>
                    </div>
                    <div class="stat-item">
                      <span class="stat-value">{{ user.successRate }}%</span>
                      <span class="stat-label">Успех</span>
                    </div>
                    <div class="stat-item">
                      <span class="stat-value">{{ user.rank }}</span>
                      <span class="stat-label">Место</span>
                    </div>
                  </div>

                  <!-- Языки программирования -->
                  <div class="user-languages">
                    <h4>Основные языки:</h4>
                    <div class="languages-tags">
                      <span
                          v-for="lang in user.topLanguages"
                          :key="lang.name"
                          class="language-tag"
                          :style="{ backgroundColor: lang.color }"
                      >
                        <span class="lang-icon-small">{{ lang.icon }}</span>
                        {{ lang.name }}
                      </span>
                    </div>
                  </div>

                  <!-- Последняя активность -->
                  <div class="user-activity">
                    <div class="activity-info">
                      <span class="activity-label">Последняя активность:</span>
                      <span class="activity-time">{{ formatTime(user.lastActive) }}</span>
                    </div>
                    <div class="recent-task" v-if="user.recentTask">
                      Решил: "{{ user.recentTask }}"
                    </div>
                  </div>

                  <!-- Достижения -->
                  <div class="user-achievements" v-if="user.achievements.length > 0">
                    <h4>Достижения:</h4>
                    <div class="achievements-list">
                      <div
                          v-for="achievement in user.achievements.slice(0, 3)"
                          :key="achievement.id"
                          class="achievement-badge"
                          :title="achievement.description"
                      >
                        <span class="achievement-icon">{{ achievement.icon }}</span>
                      </div>
                      <div
                          class="achievement-more"
                          v-if="user.achievements.length > 3"
                          :title="`Еще ${user.achievements.length - 3} достижений`"
                      >
                        +{{ user.achievements.length - 3 }}
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Пагинация -->
              <div class="pagination-controls retro-card" v-if="totalPages > 1">
                <div class="pagination-info">
                  Страница {{ currentPage }} из {{ totalPages }}
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

              <!-- Сообщение при пустом результате -->
              <div class="empty-state retro-card" v-if="filteredUsers.length === 0">
                <h3>Пользователи не найдены</h3>
                <p>Попробуйте изменить параметры поиска или фильтры</p>
                <button @click="resetFilters" class="btn-primary">
                  Сбросить фильтры
                </button>
              </div>
            </main>
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

export default {
  name: 'UsersStatistics',
  components: {
    AppNavigation,
    DangerousHTML,
    AppFooter,
  },
  data() {
    return {
      viewMode: 'grid',
      sortBy: 'rating',
      languageFilter: '',
      levelFilter: '',
      searchTerm: '',
      currentPage: 1,
      pageSize: 12,

      leaderboard: [],
      users: [],
      communityStats: {
        totalUsers: 0,
        totalTasks: 0,
        activeToday: 0,
        countries: 0
      },
      popularLanguages: [],

      availableLanguages: [
        { id: 'python', name: 'Python', icon: '🐍' },
        { id: 'java', name: 'Java', icon: '☕' },
        { id: 'javascript', name: 'JavaScript', icon: '📜' },
        { id: 'cpp', name: 'C++', icon: '⚡' },
        { id: 'csharp', name: 'C#', icon: '🎵' },
        { id: 'go', name: 'Go', icon: '🐹' },
        { id: 'rust', name: 'Rust', icon: '🦀' }
      ]
    }
  },
  computed: {
    hasActiveFilters() {
      return this.languageFilter || this.levelFilter || this.searchTerm
    },
    filteredUsers() {
      let filtered = this.users

      // Поиск по имени или био
      if (this.searchTerm) {
        const term = this.searchTerm.toLowerCase()
        filtered = filtered.filter(user =>
            user.name.toLowerCase().includes(term) ||
            (user.bio && user.bio.toLowerCase().includes(term))
        )
      }

      // Фильтр по языку
      if (this.languageFilter) {
        filtered = filtered.filter(user =>
            user.topLanguages.some(lang => lang.name.toLowerCase() === this.languageFilter)
        )
      }

      // Фильтр по уровню
      if (this.levelFilter) {
        filtered = filtered.filter(user => user.level === this.levelFilter)
      }

      // Сортировка
      filtered.sort((a, b) => {
        switch (this.sortBy) {
          case 'rating':
            return b.rating - a.rating
          case 'tasks':
            return b.completedTasks - a.completedTasks
          case 'recent':
            return new Date(b.lastActive) - new Date(a.lastActive)
          case 'name':
            return a.name.localeCompare(b.name)
          default:
            return 0
        }
      })

      return filtered
    },

    paginatedUsers() {
      const start = (this.currentPage - 1) * this.pageSize
      return this.filteredUsers.slice(start, start + this.pageSize)
    },

    totalPages() {
      return Math.ceil(this.filteredUsers.length / this.pageSize)
    },

    visiblePages() {
      const pages = []
      const start = Math.max(1, this.currentPage - 2)
      const end = Math.min(this.totalPages, start + 4)

      for (let i = start; i <= end; i++) {
        pages.push(i)
      }
      return pages
    }
  },
  async mounted() {
    await this.loadData()
  },
  watch: {
    sortBy() {
      this.currentPage = 1
    },
    languageFilter() {
      this.currentPage = 1
    },
    levelFilter() {
      this.currentPage = 1
    },
    searchTerm() {
      this.currentPage = 1
    }
  },
  methods: {

    getLanguageName(langId) {
      const lang = this.availableLanguages.find(l => l.id === langId)
      return lang ? lang.name : langId
    },

    async loadData() {
      // Загрузка лидерборда
      this.leaderboard = await this.fetchLeaderboard()

      // Загрузка пользователей
      this.users = await this.fetchUsers()

      // Загрузка статистики
      this.communityStats = await this.fetchCommunityStats()

      // Загрузка популярных языков
      this.popularLanguages = await this.fetchPopularLanguages()
    },

    async fetchLeaderboard() {
      // Имитация API запроса
      return [
        {
          id: 1,
          name: 'Алексей Петров',
          avatar: '',
          rating: 2845,
          completedTasks: 156,
          progress: 85
        },
        {
          id: 2,
          name: 'Мария Иванова',
          avatar: '',
          rating: 2678,
          completedTasks: 142,
          progress: 78
        },
        {
          id: 3,
          name: 'Дмитрий Сидоров',
          avatar: '',
          rating: 2543,
          completedTasks: 134,
          progress: 72
        },
        {
          id: 4,
          name: 'Екатерина Козлова',
          avatar: '',
          rating: 2432,
          completedTasks: 128,
          progress: 68
        },
        {
          id: 5,
          name: 'Сергей Николаев',
          avatar: '',
          rating: 2387,
          completedTasks: 121,
          progress: 65
        }
      ]
    },

    async fetchUsers() {
      // Имитация API запроса
      return Array.from({ length: 50 }, (_, i) => ({
        id: i + 1,
        name: `Пользователь ${i + 1}`,
        bio: i % 3 === 0 ? 'Люблю решать алгоритмические задачи и изучать новые технологии' :
            i % 3 === 1 ? 'Full-stack разработчик с опытом в веб-приложениях' :
                'Студент компьютерных наук, увлекаюсь машинным обучением',
        avatar: i % 5 === 0 ? '/avatars/user' + (i + 1) + '.jpg' : '',
        country: ['Россия', 'Украина', 'Беларусь', 'Казахстан'][i % 4],
        level: ['beginner', 'intermediate', 'advanced', 'expert'][i % 4],
        rating: 2500,
        completedTasks: 20 + Math.floor(Math.random() * 150),
        successRate: 60 + Math.floor(Math.random() * 35),
        rank: i + 1,
        isOnline: Math.random() > 0.7,
        lastActive: new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000),
        recentTask: ['Сортировка пузырьком', 'Поиск в глубину', 'Динамическое программирование'][i % 3],
        topLanguages: [
          { name: 'python', icon: '🐍', color: '#3572A5' },
          { name: 'java', icon: '☕', color: '#B07219' },
          { name: 'javascript', icon: '📜', color: '#F1E05A' }
        ].slice(0, 1 + i % 3),
        achievements: Array.from({ length: 2 + i % 5 }, (_, j) => ({
          id: j,
          icon: ['🏆', '⭐', '🚀', '💡', '🔧'][j % 5],
          description: 'Достижение ' + (j + 1)
        }))
      }))
    },

    async fetchCommunityStats() {
      return {
        totalUsers: 1247,
        totalTasks: 45632,
        activeToday: 187,
        countries: 24
      }
    },

    async fetchPopularLanguages() {
      return [
        { name: 'Python', icon: '🐍', percentage: 35, color: 'python' },
        { name: 'Java', icon: '☕', percentage: 25, color: 'java' },
        { name: 'JavaScript', icon: '📜', percentage: 20, color: 'javascript' },
        { name: 'C++', icon: '⚡', percentage: 12, color: 'cpp' },
        { name: 'Go', icon: '🐹', percentage: 8, color: 'go' }
      ]
    },

    getRankColor(index) {
      const colors = ['var(--color-accent)', 'var(--color-primary)', 'var(--color-secondary)', '#6B7280', '#9CA3AF']
      return colors[Math.min(index, colors.length - 1)]
    },

    getLevelIcon(level) {
      const icons = {
        beginner: '🌱',
        intermediate: '🎯',
        advanced: '🚀',
        expert: '🏆'
      }
      return icons[level] || '💼'
    },

    getLevelLabel(level) {
      const labels = {
        beginner: 'Начинающий',
        intermediate: 'Средний',
        advanced: 'Продвинутый',
        expert: 'Эксперт'
      }
      return labels[level] || level
    },

    formatTime(date) {
      const now = new Date()
      const diff = now - new Date(date)
      const days = Math.floor(diff / (1000 * 60 * 60 * 24))

      if (days === 0) return 'сегодня'
      if (days === 1) return 'вчера'
      if (days < 7) return `${days} дней назад`
      if (days < 30) return `${Math.floor(days / 7)} недель назад`
      return `${Math.floor(days / 30)} месяцев назад`
    },

    viewProfile(userId) {
      this.$router.push(`/profile/${userId}`)
    },

    sendMessage(userId) {
      console.log('Отправка сообщения пользователю:', userId)
    },

    refreshLeaderboard() {
      this.loadData()
    },

    resetFilters() {
      this.searchTerm = ''
      this.languageFilter = ''
      this.levelFilter = ''
      this.sortBy = 'rating'
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
.users-stats-container {
  width: 100%;
  display: block;
  min-height: 100vh;
  font-family: var(--font-family-body);
  background: var(--color-surface);
  position: relative;
}

.users-stats-wrapper {
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

/* Быстрые фильтры */
.quick-filters {
  padding: var(--spacing-lg);
}

.filters-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--spacing-lg);
  align-items: end;
}

.filter-group label,
.search-group label {
  display: block;
  margin-bottom: var(--spacing-sm);
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
  font-size: var(--font-size-sm);
}

.search-input {
  display: flex;
  align-items: center;
  padding: var(--spacing-sm);
  gap: var(--spacing-sm);
}

.search-input input {
  border: none;
  background: transparent;
  flex: 1;
  font-size: var(--font-size-base);
  color: var(--color-on-surface);
}

.search-input input:focus {
  outline: none;
}

.search-icon {
  font-size: var(--font-size-base);
  color: var(--color-on-surface-secondary);
}

/* Основной лейаут */
.content-layout {
  display: grid;
  grid-template-columns: 320px 1fr;
  gap: var(--spacing-xl);
  align-items: start;
  margin-bottom: var(--spacing-2xl);
}

.sidebar-panel {
  position: sticky;
  top: var(--spacing-xl);
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

/* Лидерборд */
.leaderboard-section {
  padding: var(--spacing-lg);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-lg);
}

.section-header h2 {
  margin: 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.header-icon {
  font-size: var(--font-size-base);
}

.leaderboard-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.leaderboard-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  border-radius: var(--border-radius-md);
  background: var(--color-backplate);
  border: 1px solid var(--color-border);
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
}

.leaderboard-item:hover {
  transform: translateX(var(--spacing-xs));
  box-shadow: var(--shadow-level-1);
}

.leaderboard-item.rank-1 {
  background: linear-gradient(135deg, color-mix(in srgb, var(--color-accent) 15%, transparent), transparent);
  border-color: var(--color-accent);
}

.leaderboard-item.rank-2 {
  background: linear-gradient(135deg, color-mix(in srgb, var(--color-primary) 12%, transparent), transparent);
  border-color: var(--color-primary);
}

.leaderboard-item.rank-3 {
  background: linear-gradient(135deg, color-mix(in srgb, var(--color-secondary) 10%, transparent), transparent);
  border-color: var(--color-secondary);
}

.rank-badge {
  position: relative;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-surface);
  border: 2px solid var(--color-border);
  border-radius: 50%;
  font-weight: var(--font-weight-heading);
  font-size: var(--font-size-sm);
}

.rank-crown {
  position: absolute;
  top: -8px;
  right: -8px;
  font-size: var(--font-size-sm);
}

.user-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  overflow: hidden;
  background: var(--color-primary);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--color-on-primary);
  font-weight: var(--font-weight-heading);
  font-size: var(--font-size-base);
}

.user-avatar img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.user-info {
  flex: 1;
}

.user-name {
  margin: 0 0 var(--spacing-xs) 0;
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.user-stats {
  display: flex;
  gap: var(--spacing-md);
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
}

.rating {
  color: var(--color-accent);
  font-weight: var(--font-weight-heading);
}

.tasks {
  color: var(--color-primary);
  font-weight: var(--font-weight-heading);
}

.progress-ring {
  flex-shrink: 0;
}

/* Глобальная статистика */
.global-stats {
  padding: var(--spacing-lg);
}

.global-stats h2 {
  margin: 0 0 var(--spacing-lg) 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.stats-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-md);
}

.stat-card {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  background: var(--color-backplate);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
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
  text-transform: lowercase;
}

/* Языки программирования */
.languages-stats {
  padding: var(--spacing-lg);
}

.languages-stats h2 {
  margin: 0 0 var(--spacing-lg) 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.languages-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.language-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-sm);
}

.lang-info {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.lang-icon {
  font-size: var(--font-size-base);
}

.lang-name {
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
  font-size: var(--font-size-sm);
}

.lang-stats {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  min-width: 80px;
}

.lang-percentage {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  font-weight: var(--font-weight-heading);
  min-width: 30px;
  text-align: right;
}

.progress-bar {
  width: 60px;
  height: 6px;
  background: var(--color-border);
  border-radius: var(--border-radius-full);
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  border-radius: var(--border-radius-full);
  transition: width var(--animation-duration-slow) var(--animation-curve-primary);
}

.progress-fill.python { background: #3572A5; }
.progress-fill.java { background: #B07219; }
.progress-fill.javascript { background: #F1E05A; }
.progress-fill.cpp { background: #F34B7D; }
.progress-fill.go { background: #00ADD8; }

/* Основное содержимое */
.main-content {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.content-header {
  padding: var(--spacing-lg);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-left h2 {
  margin: 0 0 var(--spacing-xs) 0;
  font-size: var(--font-size-xl);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.users-count {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
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

/* Контейнер пользователей */
.users-container {
  display: grid;
  gap: var(--spacing-lg);
}

.users-container.grid {
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
}

.users-container.list {
  grid-template-columns: 1fr;
}

/* Карточка пользователя */
.user-card {
  padding: var(--spacing-lg);
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
}

.user-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-level-2);
}

.user-header {
  display: flex;
  gap: var(--spacing-lg);
  margin-bottom: var(--spacing-lg);
}

.user-avatar-large {
  position: relative;
  width: 80px;
  height: 80px;
  border-radius: 50%;
  overflow: hidden;
  background: var(--color-primary);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--color-on-primary);
  font-weight: var(--font-weight-heading);
  font-size: var(--font-size-xl);
  flex-shrink: 0;
}

.user-avatar-large img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.online-indicator {
  position: absolute;
  bottom: 4px;
  right: 4px;
  width: 16px;
  height: 16px;
  border-radius: 50%;
  background: var(--color-border);
  border: 2px solid var(--color-surface);
}

.online-indicator.online {
  background: var(--color-accent);
}

.user-main-info {
  flex: 1;
}

.user-name {
  margin: 0 0 var(--spacing-sm) 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.user-bio {
  margin: 0 0 var(--spacing-md) 0;
  color: var(--color-on-surface-secondary);
  font-size: var(--font-size-sm);
  line-height: var(--line-height-body);
}

.user-meta {
  display: flex;
  gap: var(--spacing-lg);
  font-size: var(--font-size-sm);
}

.user-country,
.user-level {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
  color: var(--color-on-surface-secondary);
}

.user-level.beginner { color: #10B981; }
.user-level.intermediate { color: #3B82F6; }
.user-level.advanced { color: #8B5CF6; }
.user-level.expert { color: #EF4444; }

.meta-icon {
  font-size: var(--font-size-base);
}

.user-actions {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
  align-self: flex-start;
}

/* Статистика пользователя */
.user-stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: var(--spacing-md);
  margin-bottom: var(--spacing-lg);
  padding: var(--spacing-md);
  background: var(--color-backplate);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
}

.stat-item {
  text-align: center;
}

.stat-value {
  display: block;
  font-size: var(--font-size-lg);
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

/* Языки пользователя */
.user-languages {
  margin-bottom: var(--spacing-lg);
}

.user-languages h4 {
  margin: 0 0 var(--spacing-sm) 0;
  font-size: var(--font-size-base);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.languages-tags {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-sm);
}

.language-tag {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius-full);
  font-size: var(--font-size-sm);
  color: white;
  font-weight: var(--font-weight-heading);
}

.lang-icon-small {
  font-size: var(--font-size-sm);
}

/* Активность пользователя */
.user-activity {
  margin-bottom: var(--spacing-lg);
  padding: var(--spacing-md);
  background: var(--color-backplate);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
}

.activity-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-sm);
  font-size: var(--font-size-sm);
}

.activity-label {
  color: var(--color-on-surface-secondary);
}

.activity-time {
  color: var(--color-on-surface);
  font-weight: var(--font-weight-heading);
}

.recent-task {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  font-size: var(--font-size-sm);
  color: var(--color-on-surface);
}

.task-icon {
  font-size: var(--font-size-base);
}

/* Достижения */
.user-achievements h4 {
  margin: 0 0 var(--spacing-sm) 0;
  font-size: var(--font-size-base);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.achievements-list {
  display: flex;
  gap: var(--spacing-sm);
  align-items: center;
}

.achievement-badge {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: var(--color-accent);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: var(--font-size-sm);
  color: var(--color-on-surface);
  border: 2px solid var(--color-surface);
  box-shadow: var(--shadow-level-1);
}

.achievement-more {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: var(--color-border);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  font-weight: var(--font-weight-heading);
  border: 2px solid var(--color-surface);
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
  .content-layout {
    grid-template-columns: 280px 1fr;
    gap: var(--spacing-lg);
  }

  .users-container.grid {
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  }
}

@media (max-width: 1024px) {
  .content-layout {
    grid-template-columns: 1fr;
  }

  .sidebar-panel {
    position: static;
    order: 2;
  }

  .filters-grid {
    grid-template-columns: 1fr 1fr;
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

  .users-container.grid {
    grid-template-columns: 1fr;
  }

  .user-header {
    flex-direction: column;
    text-align: center;
    gap: var(--spacing-md);
  }

  .user-actions {
    flex-direction: row;
    justify-content: center;
  }

  .user-stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .pagination-controls {
    flex-direction: column;
    gap: var(--spacing-md);
    text-align: center;
  }

  .stats-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 480px) {
  .user-meta {
    flex-direction: column;
    gap: var(--spacing-sm);
  }

  .view-controls {
    width: 100%;
    justify-content: center;
  }

  .page-numbers {
    display: none;
  }
}
/* Стили для улучшенных фильтров */
.quick-filters {
  padding: var(--spacing-lg);
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

/* Стили для поиска */
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

/* Адаптивность */
@media (max-width: 768px) {
  .filters-grid {
    grid-template-columns: 1fr;
    gap: var(--spacing-md);
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
}
</style>