<template>
  <div class="student-profile-container">
    <app-navigation></app-navigation>

    <div class="student-profile-wrapper">
      <DangerousHTML
          html="<style>
  .student-profile-container {
    min-height: 100vh;
    background: var(--color-surface);
    padding: var(--spacing-2xl) 0;
  }

  .student-profile-container::before {
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

  .profile-section {
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

      <section class="student-profile-section" role="main" aria-label="Профиль студента">
        <div class="container">
          <!-- Хлебные крошки -->
          <nav class="breadcrumbs" aria-label="Навигация">
            <ol class="breadcrumbs-list">
              <li class="breadcrumb-item">
                <a href="/" class="breadcrumb-link">Главная</a>
              </li>
              <li class="breadcrumb-separator">/</li>
              <li class="breadcrumb-item">
                <span class="breadcrumb-current">Мой профиль</span>
              </li>
            </ol>
          </nav>

          <div class="profile-layout">
            <!-- Боковая панель профиля -->
            <aside class="profile-sidebar" role="complementary" aria-label="Информация профиля">
              <!-- Аватар и основная информация -->
              <div class="profile-card retro-card">
                <div class="profile-header">
                  <div class="avatar-section">
                    <div class="avatar-container">
                      <img
                          :src="userProfile.avatar"
                          :alt="userProfile.name"
                          class="profile-avatar"
                          v-if="userProfile.avatar"
                      >
                      <div class="avatar-placeholder" v-else>
                        {{ userProfile.userName.charAt(0).toUpperCase() }}
                      </div>
                      <div class="online-status" :class="{ online: userProfile.isOnline }">
                        {{ userProfile.isOnline ? 'Онлайн' : 'Не в сети' }}
                      </div>
                    </div>

                  </div>

                  <div class="profile-basic-info">
                    <h1 class="profile-name">{{ userProfile.name }}</h1>
                    <p class="profile-username">@{{ userProfile.userName }}</p>

                    <div class="profile-badges">
                      <span class="level-badge" :class="userProfile.level">
                        <span class="badge-icon">{{ getLevelIcon(userProfile.level) }}</span>
                        {{ getLevelLabel(userProfile.level) }}
                      </span>
                      <span class="rating-badge">
                        <span class="badge-icon">⭐</span>
                        Рейтинг: {{ userProfile.rating }}
                      </span>
                    </div>

                    <div class="profile-meta">
                      <div class="meta-item">

                        <span class="meta-text">Участник с {{ formatDate(userProfile.joinDate) }}</span>
                      </div>
                      <div class="meta-item" v-if="userProfile.country">
                        <span class="meta-text">{{ userProfile.country }}</span>
                      </div>
                      <div class="meta-item" v-if="userProfile.company">
                        <span class="meta-text">{{ userProfile.company }}</span>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Статистика профиля -->
                <div class="profile-stats">
                  <div class="stat-item">
                    <div class="stat-value">{{ userProfile.completedTasks }}</div>
                    <div class="stat-label">Решено задач</div>
                  </div>
                  <div class="stat-item">
                    <div class="stat-value">{{ userProfile.successRate }}%</div>
                    <div class="stat-label">Успешность</div>
                  </div>
                  <div class="stat-item">
                    <div class="stat-value">#{{ userProfile.rank }}</div>
                    <div class="stat-label">Место в рейтинге</div>
                  </div>
                </div>

                <!-- Прогресс уровня -->
                <div class="level-progress">
                  <div class="progress-header">
                    <span class="progress-label">Прогресс до {{ getNextLevel(userProfile.level) }}</span>
                    <span class="progress-percentage">{{ userProfile.levelProgress }}%</span>
                  </div>
                  <div class="progress-bar">
                    <div
                        class="progress-fill"
                        :style="{ width: userProfile.levelProgress + '%' }"
                    ></div>
                  </div>
                </div>
              </div>

              <!-- Контакты и социальные сети -->
              <div class="contacts-card retro-card">
                <h3 class="card-title">
                  Контакты
                </h3>

                <div class="contacts-list">
                  <div class="contact-item" v-if="userProfile.email">
                    <a :href="'mailto:' + userProfile.email" class="contact-link">
                      {{ userProfile.email }}
                    </a>
                  </div>
                  <div class="contact-item" v-if="userProfile.githubUrl">
                    <a :href="userProfile.githubUrl" target="_blank" class="contact-link">
                      GitHub
                    </a>
                  </div>
                  <div class="contact-item" v-if="userProfile.linkedinUrl">
                    <a :href="userProfile.linkedinUrl" target="_blank" class="contact-link">
                      LinkedIn
                    </a>
                  </div>
                  <div class="contact-item" v-if="userProfile.website">
                    <a :href="userProfile.website" target="_blank" class="contact-link">
                      Веб-сайт
                    </a>
                  </div>
                </div>
              </div>

            </aside>

            <!-- Основное содержимое профиля -->
            <main class="profile-main" role="region" aria-label="Основная информация профиля">
              <!-- Навигация по вкладкам -->
              <div class="profile-tabs retro-card">
                <nav class="tabs-navigation" role="tablist">
                  <button
                      v-for="tab in tabs"
                      :key="tab.id"
                      :class="['tab-btn', { active: activeTab === tab.id }]"
                      @click="activeTab = tab.id"
                      :aria-selected="activeTab === tab.id"
                      role="tab"
                  >
                    {{ tab.name }}
                    <span class="tab-badge" v-if="tab.badge">{{ tab.badge }}</span>
                  </button>
                </nav>
              </div>

              <!-- Содержимое вкладок -->
              <div class="tab-content">
                <!-- Вкладка: Обзор -->
                <div v-if="activeTab === 'overview'" class="tab-pane profile-section">
                  <!-- Биография -->
                  <div class="bio-card retro-card">
                    <div class="card-header">
                      <h2 class="card-title">
                        О себе
                      </h2>
                    </div>

                    <div class="bio-content">
                      <p v-if="userProfile.bio" class="bio-text">{{ userProfile.bio }}</p>
                      <div v-else class="bio-empty">
                        <p>Расскажите о себе, чтобы другие участники могли узнать вас лучше</p>
                      </div>
                    </div>
                  </div>

                  <!-- Последняя активность -->
                  <div class="activity-card retro-card">
                    <h2 class="card-title">
                      Последняя активность
                    </h2>

                    <div class="activity-list">
                      <div
                          v-for="activity in recentActivities"
                          :key="activity.id"
                          class="activity-item"
                      >
                        <div class="activity-icon">
                          {{ activity.icon }}
                        </div>
                        <div class="activity-content">
                          <p class="activity-text">{{ activity.text }}</p>
                          <span class="activity-time">{{ formatTime(activity.timestamp) }}</span>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Достижения -->
                  <div class="achievements-card retro-card">
                    <div class="card-header">
                      <h2 class="card-title">
                        Достижения
                        <span class="achievements-count">({{ userProfile.achievements.length }})</span>
                      </h2>
                    </div>

                    <div class="achievements-grid">
                      <div
                          v-for="achievement in userProfile.achievements.slice(0, 6)"
                          :key="achievement.id"
                          class="achievement-item"
                          :title="achievement.description"
                      >
                        <div class="achievement-icon">
                          {{ achievement.icon }}
                        </div>
                        <div class="achievement-info">
                          <h4 class="achievement-name">{{ achievement.name }}</h4>
                          <p class="achievement-description">{{ achievement.description }}</p>
                          <span class="achievement-date">{{ formatDate(achievement.date) }}</span>
                        </div>
                      </div>
                    </div>

                    <div class="achievements-empty" v-if="userProfile.achievements.length === 0">
                      <p>Здесь будут отображаться ваши достижения</p>
                      <p class="empty-hint">Решайте задачи и участвуйте в соревнованиях, чтобы получать достижения!</p>
                    </div>
                  </div>
                </div>

                <!-- Вкладка: Статистика -->
                <div v-if="activeTab === 'stats'" class="tab-pane profile-section">
                  <!-- Общая статистика -->
                  <div class="stats-overview retro-card">
                    <h2 class="card-title">
                      Общая статистика
                    </h2>

                    <div class="stats-grid">
                      <div class="stat-card">
                        <div class="stat-data">
                          <span class="stat-value">{{ userStats.totalSolved }}</span>
                          <span class="stat-label">Всего решено</span>
                        </div>
                      </div>
                      <div class="stat-card">
                        <div class="stat-data">
                          <span class="stat-value">{{ userStats.averageTime }}м</span>
                          <span class="stat-label">Среднее время</span>
                        </div>
                      </div>
                      <div class="stat-card">
                        <div class="stat-data">
                          <span class="stat-value">{{ userStats.successRate }}%</span>
                          <span class="stat-label">Успешность</span>
                        </div>
                      </div>
                      <div class="stat-card">
                        <div class="stat-data">
                          <span class="stat-value">{{ userStats.streak }}</span>
                          <span class="stat-label">Дней подряд</span>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Статистика по сложности -->
                  <div class="difficulty-stats retro-card">
                    <h2 class="card-title">
                      Статистика по сложности
                    </h2>

                    <div class="difficulty-grid">
                      <div
                          v-for="diff in difficultyStats"
                          :key="diff.level"
                          class="difficulty-item"
                      >
                        <div class="diff-header">
                          <span class="diff-icon">{{ diff.icon }}</span>
                          <span class="diff-name">{{ diff.name }}</span>
                        </div>
                        <div class="diff-progress">
                          <div class="progress-info">
                            <span class="progress-value">{{ diff.solved }}/{{ diff.total }}</span>
                            <span class="progress-percentage">{{ diff.percentage }}%</span>
                          </div>
                          <div class="progress-bar">
                            <div
                                class="progress-fill"
                                :style="{ width: diff.percentage + '%' }"
                                :class="diff.level"
                            ></div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- График активности -->
                  <div class="activity-chart retro-card">
                    <h2 class="card-title">
                      Активность за последние 30 дней
                    </h2>

                    <div class="chart-container">
                      <div class="activity-calendar">
                        <div
                            v-for="day in activityCalendar"
                            :key="day.date"
                            class="calendar-day"
                            :class="day.intensity"
                            :title="`${day.date}: ${day.count} задач`"
                        ></div>
                      </div>
                    </div>

                    <div class="chart-legend">
                      <div class="legend-item">
                        <span class="legend-color less"></span>
                        <span>Меньше активности</span>
                      </div>
                      <div class="legend-item">
                        <span class="legend-color more"></span>
                        <span>Больше активности</span>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Вкладка: Решенные задачи -->
                <div v-if="activeTab === 'solved'" class="tab-pane profile-section">
                  <div class="solved-tasks-header retro-card">
                    <div class="header-content">
                      <h2 class="card-title">
                        Решенные задачи
                        <span class="tasks-count">({{ solvedTasks.length }})</span>
                      </h2>

                      <div class="tasks-filters">
                        <select v-model="taskFilter" class="vintage-border">
                          <option value="all">Все задачи</option>
                          <option value="easy">Легкие</option>
                          <option value="medium">Средние</option>
                          <option value="hard">Сложные</option>
                        </select>

                        <select v-model="taskSort" class="vintage-border">
                          <option value="recent">По дате</option>
                          <option value="difficulty">По сложности</option>
                          <option value="language">По языку</option>
                        </select>
                      </div>
                    </div>
                  </div>

                  <div class="solved-tasks-list">
                    <div
                        v-for="task in filteredSolvedTasks"
                        :key="task.id"
                        class="task-item retro-card"
                    >
                      <div class="task-main">
                        <div class="task-info">
                          <h3 class="task-title">{{ task.title }}</h3>
                          <div class="task-meta">
                            <span class="task-difficulty" :class="task.difficulty">
                              {{ getDifficultyLabel(task.difficulty) }}
                            </span>
                            <span class="task-language">
                              <span class="lang-icon">{{ getLanguageIcon(task.language) }}</span>
                              {{ task.language }}
                            </span>
                            <span class="task-time">
                              {{ task.timeSpent }}м
                            </span>
                          </div>
                        </div>

                        <div class="task-actions">
                          <button @click="reattemptTask(task.id)" class="btn-text btn-sm">
                          </button>
                        </div>
                      </div>

                      <div class="task-stats">
                        <div class="stat">
                          <span class="stat-label">Попыток:</span>
                          <span class="stat-value">{{ task.attempts }}</span>
                        </div>
                        <div class="stat">
                          <span class="stat-label">Время:</span>
                          <span class="stat-value">{{ task.bestTime }}мс</span>
                        </div>
                        <div class="stat">
                          <span class="stat-label">Решено:</span>
                          <span class="stat-value">{{ formatDate(task.solvedAt) }}</span>
                        </div>
                      </div>
                    </div>

                    <div class="empty-state" v-if="filteredSolvedTasks.length === 0">
                      <h3>Задачи не найдены</h3>
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
  </div>
</template>

<script>
import DangerousHTML from 'dangerous-html/vue'
import AppNavigation from '../components/navigation'
import AppFooter from '../components/footer'
import { userProfilesAPI, userProfileHelpers, USER_PROFILE_CONSTANTS } from '../api/user'

export default {
  name: 'StudentProfile',
  components: {
    AppNavigation,
    DangerousHTML,
    AppFooter,
  },
  data() {
    return {
      activeTab: 'overview',
      taskFilter: 'all',
      taskSort: 'recent',
      isLoading: false,
      error: null,

      tabs: [
        { id: 'overview', name: 'Обзор', badge: null },
        { id: 'stats', name: 'Статистика', badge: null },
        { id: 'solved', name: 'Решенные задачи', badge: null }
      ],

      userProfile: {
        id: '',
        userId: '',
        userName: '',
        avatarUrl: '',
        bio: '',
        country: '',
        gitHubUrl: '',
        linkedInUrl: '',
        level: '',
        stats: {},
        settings: {},
        isPublic: true,
        achievements: [],
        recentBattles: [],
        recentActivities: [],
        recentProblems: [],
        createdAt: ''
      },

      userStats: {
        totalProblemsSolved: 0,
        totalBattles: 0,
        wins: 0,
        losses: 0,
        draws: 0,
        currentStreak: 0,
        maxStreak: 0,
        totalExperience: 0,
        winRate: 0,
        experienceToNextLevel: 0,
        easyProblemsSolved: 0,
        mediumProblemsSolved: 0,
        hardProblemsSolved: 0,
        totalSubmissions: 0,
        successfulSubmissions: 0,
        totalExecutionTime: '0:0:0',
        successRate: 0,
        averageExecutionTime: '00:00:0'
      },

      difficultyStats: [
        { level: 'easy', name: 'Легкие', icon: '🌱', solved: 0, total: 0, percentage: 0 },
        { level: 'medium', name: 'Средние', icon: '🎯', solved: 0, total: 0, percentage: 0 },
        { level: 'hard', name: 'Сложные', icon: '🚀', solved: 0, total: 0, percentage: 0 }
      ],

      solvedTasks: [],
      recentActivities: [],
      userSettings: {},
      activityCalendar: []
    }
  },
  computed: {
    filteredSolvedTasks() {
      let tasks = this.solvedTasks

      if (this.taskFilter !== 'all') {
        tasks = tasks.filter(task => task.difficulty === this.taskFilter)
      }

      tasks.sort((a, b) => {
        switch (this.taskSort) {
          case 'recent':
            return new Date(b.solvedAt) - new Date(a.solvedAt)
          case 'difficulty':
            const difficultyOrder = { easy: 1, medium: 2, hard: 3 }
            return difficultyOrder[b.difficulty] - difficultyOrder[a.difficulty]
          case 'language':
            return a.language.localeCompare(b.language)
          default:
            return 0
        }
      })

      return tasks
    },

    // Вычисляемые свойства для удобства отображения
    profileDisplayName() {
      return this.userProfile.userName || 'Анонимный пользователь'
    },

    profileLevel() {
      return this.calculateLevel(this.userProfile.stats?.totalExperience || 0)
    },

    profileRating() {
      return this.userProfile.stats?.totalExperience || 0
    },

    completedTasksCount() {
      return this.userProfile.stats?.totalProblemsSolved || 0
    },

    successRate() {
      return Math.round((this.userProfile.stats?.successRate || 0))
    },

    currentStreak() {
      return this.userProfile.stats?.currentStreak || 0
    },

    levelProgress() {
      return this.calculateLevelProgress(
          this.userProfile.stats?.totalExperience || 0,
          this.userProfile.stats?.experienceToNextLevel || 1000
      )
    },

    isUserOnline() {
      return Math.random() > 0.3
    }
  },
  async mounted() {
    await this.loadUserProfile()
    this.updateTabBadges()
  },
  methods: {
    async loadUserProfile() {
      this.isLoading = true
      this.error = null

      try {
        // Получаем ID текущего пользователя из хранилища или другого источника
        const currentUserId = this.getCurrentUserId()

        if (!currentUserId) {
          throw new Error('ID пользователя не найден')
        }

        // Загрузка основного профиля по ID
        const profileData = await userProfilesAPI.getProfileById(currentUserId)
        this.userProfile = profileData

        this.recentActivities = profileData.recentActivities || []
        this.solvedTasks = this.mapRecentProblemsToTasks(profileData.recentProblems || [])

        // Загрузка настроек
        this.userSettings = userProfileHelpers.settingsToForm(profileData.settings || {})
        this.userSettings.isPublic = profileData.isPublic

        // Инициализация статистики
        this.initializeStats(profileData.stats || {})

        // Инициализация календаря активности
        this.initializeActivityCalendar()

      } catch (error) {
        console.error('Ошибка загрузки профиля:', error)
        this.error = 'Не удалось загрузить данные профиля. Попробуйте обновить страницу.'
        this.loadFallbackData()
      } finally {
        this.isLoading = false
      }
    },

    getCurrentUserId() {
      // Получаем ID пользователя из различных источников
      // 1. Из параметров маршрута
      if (this.$route.params.userId) {
        return this.$route.params.userId
      }

      // 2. Из локального хранилища (если там хранится ID текущего пользователя)
      const storedUserId = localStorage.getItem('currentUserId')
      if (storedUserId) {
        return storedUserId
      }

      // 3. Из данных профиля (если уже загружен)
      if (this.userProfile.userId) {
        return this.userProfile.userId
      }

      // 4. Альтернативный способ - получить через getMyProfile если доступен
      return null
    },



    mapRecentProblemsToTasks(problems) {
      return problems.map(problem => ({
        id: problem.problemId,
        title: problem.title || 'Неизвестная задача',
        difficulty: problem.difficulty?.toLowerCase() || 'medium',
        language: problem.language || 'Не указан',
        timeSpent: this.formatExecutionTime(problem.executionTime),
        attempts: 1,
        solvedAt: problem.solvedAt
      }))
    },

    initializeStats(stats) {
      this.userStats = {
        totalSolved: stats.totalProblemsSolved || 0,
        averageTime: this.formatExecutionTime(stats.averageExecutionTime),
        successRate: Math.round((stats.successRate || 0) * 100),
        streak: stats.currentStreak || 0,
        totalTimeSpent: this.formatExecutionTime(stats.totalExecutionTime)
      }

      // Инициализация статистики по сложности из реальных данных
      this.difficultyStats = [
        {
          level: 'easy',
          name: 'Легкие',
          icon: '🌱',
          solved: stats.easyProblemsSolved || 0,
          total: Math.max(stats.easyProblemsSolved || 0, 10),
          percentage: stats.easyProblemsSolved ? Math.round((stats.easyProblemsSolved / Math.max(stats.easyProblemsSolved, 10)) * 100) : 0
        },
        {
          level: 'medium',
          name: 'Средние',
          icon: '🎯',
          solved: stats.mediumProblemsSolved || 0,
          total: Math.max(stats.mediumProblemsSolved || 0, 10),
          percentage: stats.mediumProblemsSolved ? Math.round((stats.mediumProblemsSolved / Math.max(stats.mediumProblemsSolved, 10)) * 100) : 0
        },
        {
          level: 'hard',
          name: 'Сложные',
          icon: '🚀',
          solved: stats.hardProblemsSolved || 0,
          total: Math.max(stats.hardProblemsSolved || 0, 10),
          percentage: stats.hardProblemsSolved ? Math.round((stats.hardProblemsSolved / Math.max(stats.hardProblemsSolved, 10)) * 100) : 0
        }
      ]
    },

    formatExecutionTime(timeString) {
      if (!timeString || timeString === '00:00:00') return '0м'

      const [hours, minutes, seconds] = timeString.split(':').map(Number)

      if (hours > 0) {
        return `${hours}ч ${minutes}м`
      } else if (minutes > 0) {
        return `${minutes}м`
      } else {
        return `${seconds}с`
      }
    },

    initializeActivityCalendar() {
      this.activityCalendar = Array.from({ length: 30 }, (_, i) => ({
        date: new Date(Date.now() - (29 - i) * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
        count: Math.floor(Math.random() * 5),
        intensity: ['none', 'low', 'medium', 'high'][Math.floor(Math.random() * 4)]
      }))
    },

    updateTabBadges() {
      this.tabs.find(tab => tab.id === 'solved').badge = this.solvedTasks.length
    },

    calculateLevel(experience) {
      const exp = experience || 0
      if (exp >= 5000) return 'expert'
      if (exp >= 2000) return 'advanced'
      if (exp >= 500) return 'intermediate'
      return 'Beginner'
    },

    calculateLevelProgress(experience, experienceToNextLevel) {
      if (!experienceToNextLevel || experienceToNextLevel === 0) return 100
      return Math.min(100, Math.max(0, (experience / experienceToNextLevel) * 100))
    },

    getLevelIcon(level) {
      const icons = {
        Beginner: '🌱',
        Intermediate: '🎯',
        Advanced: '🚀',
        Expert: '🏆'
      }
      return icons[level] || '💼'
    },

    getLevelLabel(level) {
      const labels = {
        Beginner: 'Начинающий',
        Intermediate: 'Средний',
        Advanced: 'Продвинутый',
        Expert: 'Эксперт'
      }
      return labels[level] || level
    },

    getNextLevel(level) {
      const nextLevels = {
        Beginner: 'Средний',
        Intermediate: 'Продвинутый',
        Advanced: 'Эксперт',
        Expert: 'Мастер'
      }
      return nextLevels[level] || 'Следующий уровень'
    },

    getActivityIcon(activityType) {
      const icons = {
        'task_solved': '✅',
        'battle_won': '⚔️',
        'achievement': '🏆',
        'level_up': '📈'
      }
      return icons[activityType] || '📝'
    },

    getSkillLevelText(level) {
      const levels = {
        1: 'Новичок',
        2: 'Начинающий',
        3: 'Средний',
        4: 'Продвинутый',
        5: 'Эксперт'
      }
      return levels[level] || 'Не указано'
    },

    getDifficultyLabel(difficulty) {
      const labels = {
        easy: 'Легкая',
        medium: 'Средняя',
        hard: 'Сложная'
      }
      return labels[difficulty] || difficulty
    },

    getLanguageIcon(language) {
      const icons = {
        'python': '🐍',
        'java': '☕',
        'javascript': '📜',
        'cpp': '⚡',
        'csharp': '🎵'
      }
      return icons[language] || '💻'
    },

    getUserSkills() {
      const preferredLanguage = this.userProfile.settings?.preferredLanguage
      if (!preferredLanguage) return []

      const skillMap = {
        'python': { name: 'Python', icon: '🐍', level: 4 },
        'java': { name: 'Java', icon: '☕', level: 3 },
        'javascript': { name: 'JavaScript', icon: '📜', level: 3 },
        'csharp': { name: 'C#', icon: '🎵', level: 3 },
        'cpp': { name: 'C++', icon: '⚡', level: 2 }
      }

      const mainSkill = skillMap[preferredLanguage] || { name: 'Программирование', icon: '💻', level: 3 }

      return [mainSkill]
    },

    getAchievements() {
      return this.userProfile.achievements || []
    },

    formatDate(dateString) {
      if (!dateString) return 'Не указано'
      const date = new Date(dateString)
      return date.toLocaleDateString('ru-RU', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      })
    },

    formatTime(timestamp) {
      if (!timestamp) return 'давно'
      const now = new Date()
      const date = new Date(timestamp)
      const diff = now - date
      const hours = Math.floor(diff / (1000 * 60 * 60))

      if (hours < 1) return 'только что'
      if (hours < 24) return `${hours} часов назад`

      const days = Math.floor(hours / 24)
      if (days === 1) return 'вчера'
      if (days < 7) return `${days} дней назад`

      return date.toLocaleDateString('ru-RU')
    },

    // Методы для редактирования профиля
    async editAvatar() {
      console.log('Редактирование аватара')
    },

    async editBio() {
      try {
        const newBio = prompt('Введите новое описание:', this.userProfile.bio || '')
        if (newBio !== null) {
          const updateData = {
            ...userProfileHelpers.profileToForm(this.userProfile),
            bio: newBio
          }

          await userProfilesAPI.updateMyProfile(updateData)
          await this.loadUserProfile()
        }
      } catch (error) {
        console.error('Ошибка обновления биографии:', error)
      }
    },

    async editSkills() {
      console.log('Редактирование навыков')
    },

    viewTask(taskId) {
      this.$router.push(`/tasks/${taskId}`)
    },

    reattemptTask(taskId) {
      this.$router.push(`/solve/${taskId}`)
    },

    async saveSettings() {
      try {
        await userProfilesAPI.updateMySettings(this.userSettings)

        // Обновляем публичность профиля
        await userProfilesAPI.updateMyProfile({
          ...userProfileHelpers.profileToForm(this.userProfile),
          isPublic: this.userSettings.isPublic
        })

        alert('Настройки успешно сохранены!')
        await this.loadUserProfile()
      } catch (error) {
        console.error('Ошибка сохранения настроек:', error)
        alert('Ошибка при сохранении настроек')
      }
    },

    async resetSettings() {
      try {
        const currentUserId = this.getCurrentUserId()
        if (!currentUserId) return

        const profile = await userProfilesAPI.getProfileById(currentUserId)
        this.userSettings = userProfileHelpers.settingsToForm(profile.settings || {})
        this.userSettings.isPublic = profile.isPublic
      } catch (error) {
        console.error('Ошибка сброса настроек:', error)
      }
    },

    loadFallbackData() {
      this.userProfile = {
        id: '1',
        userId: '1',
        userName: 'Иван Петров',
        avatarUrl: '',
        bio: 'Студент компьютерных наук, увлекаюсь алгоритмами и веб-разработкой.',
        country: 'Россия',
        gitHubUrl: 'https://github.com/ivanpetrov',
        linkedInUrl: 'https://linkedin.com/in/ivanpetrov',
        level: 'intermediate',
        stats: {
          totalProblemsSolved: 67,
          totalBattles: 15,
          wins: 10,
          losses: 3,
          draws: 2,
          currentStreak: 5,
          maxStreak: 12,
          totalExperience: 1845,
          winRate: 0.78,
          experienceToNextLevel: 500,
          easyProblemsSolved: 35,
          mediumProblemsSolved: 25,
          hardProblemsSolved: 7,
          totalSubmissions: 85,
          successfulSubmissions: 67,
          totalExecutionTime: '45:30:15',
          successRate: 0.78,
          averageExecutionTime: '00:25:45'
        },
        settings: {
          emailNotifications: true,
          battleInvitations: true,
          achievementNotifications: true,
          theme: 'auto',
          codeEditorTheme: 'vs-dark',
          preferredLanguage: 'python'
        },
        isPublic: true,
        achievements: [
          {
            name: 'Первая задача',
            description: 'Решил первую задачу на платформе',
            iconUrl: '',
            achievedAt: '2023-01-20T00:00:00Z'
          }
        ],
        createdAt: '2023-01-15T00:00:00Z'
      }

      this.userSettings = userProfileHelpers.settingsToForm(this.userProfile.settings)
      this.userSettings.isPublic = this.userProfile.isPublic
      this.initializeStats(this.userProfile.stats)
    }
  }
}
</script>


<style scoped>
.student-profile-container {
  width: 100%;
  display: block;
  min-height: 100vh;
  font-family: var(--font-family-body);
  background: var(--color-surface);
  position: relative;
}

.student-profile-wrapper {
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

/* Основной лейаут */
.profile-layout {
  display: grid;
  grid-template-columns: 350px 1fr;
  gap: var(--spacing-xl);
  align-items: start;
  margin-bottom: var(--spacing-2xl);
}

.profile-sidebar {
  position: sticky;
  top: var(--spacing-xl);
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

/* Карточка профиля */
.profile-card {
  padding: var(--spacing-lg);
}

.profile-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  margin-bottom: var(--spacing-lg);
}

.avatar-section {
  margin-bottom: var(--spacing-lg);
}

.avatar-container {
  position: relative;
  margin-bottom: var(--spacing-md);
}

.profile-avatar {
  width: 120px;
  height: 120px;
  border-radius: 50%;
  object-fit: cover;
  border: 4px solid var(--color-primary);
}

.avatar-placeholder {
  width: 120px;
  height: 120px;
  border-radius: 50%;
  background: var(--color-primary);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--color-on-primary);
  font-size: var(--font-size-hero);
  font-weight: var(--font-weight-heading);
  border: 4px solid var(--color-primary);
}

.online-status {
  position: absolute;
  bottom: 8px;
  right: 8px;
  background: var(--color-surface);
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius-full);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-heading);
  border: 2px solid var(--color-border);
}

.online-status.online {
  border-color: var(--color-accent);
  color: var(--color-accent);
}

.avatar-edit-btn {
  width: 100%;
}

.profile-name {
  margin: 0 0 var(--spacing-xs) 0;
  font-size: var(--font-size-xl);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.profile-username {
  margin: 0 0 var(--spacing-md) 0;
  color: var(--color-on-surface-secondary);
  font-size: var(--font-size-base);
}

.profile-badges {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
  margin-bottom: var(--spacing-lg);
}
.tasks-filters select {
  color: var(--color-on-surface);
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--border-radius-md);
  padding: var(--spacing-sm) var(--spacing-md);
  font-size: var(--font-size-base);
  cursor: pointer;
  appearance: none;
  background-image: url("data:image/svg+xml;charset=US-ASCII,<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 4 5'><path fill='%23666' d='M2 0L0 2h4zm0 5L0 3h4z'/></svg>");
  background-repeat: no-repeat;
  background-position: right var(--spacing-md) center;
  background-size: 12px;
  padding-right: var(--spacing-2xl);
}

.tasks-filters select:focus {
  outline: none;
  border-color: var(--color-primary);
}

.tasks-filters select option {
  color: var(--color-on-surface);
  background: var(--color-surface);
}

/* Для темной темы */
@media (prefers-color-scheme: dark) {
  .tasks-filters select {
    background-image: url("data:image/svg+xml;charset=US-ASCII,<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 4 5'><path fill='%23ccc' d='M2 0L0 2h4zm0 5L0 3h4z'/></svg>");
  }
}
.level-badge,
.rating-badge {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-xs);
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius-full);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-heading);
}

.level-badge {
  background: color-mix(in srgb, var(--color-primary) 15%, transparent);
  color: var(--color-primary);
  border: 1px solid var(--color-primary);
}

.rating-badge {
  background: color-mix(in srgb, var(--color-accent) 15%, transparent);
  color: var(--color-accent);
  border: 1px solid var(--color-accent);
}

.badge-icon {
  font-size: var(--font-size-base);
}

.profile-meta {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.meta-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
}

.meta-icon {
  font-size: var(--font-size-base);
}

/* Статистика профиля */
.profile-stats {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
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
  font-size: var(--font-size-xxs);
  color: var(--color-on-surface-secondary);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Прогресс уровня */
.level-progress {
  padding: var(--spacing-md);
  background: var(--color-backplate);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
}

.progress-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-sm);
  font-size: var(--font-size-sm);
}

.progress-label {
  color: var(--color-on-surface);
}

.progress-percentage {
  color: var(--color-primary);
  font-weight: var(--font-weight-heading);
}

.progress-bar {
  height: 8px;
  background: var(--color-border);
  border-radius: var(--border-radius-full);
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, var(--color-primary), var(--color-secondary));
  border-radius: var(--border-radius-full);
  transition: width var(--animation-duration-slow) var(--animation-curve-primary);
}

/* Карточки контактов и навыков */
.contacts-card,
.skills-card {
  padding: var(--spacing-lg);
}

.card-title {
  margin: 0 0 var(--spacing-lg) 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.title-icon {
  font-size: var(--font-size-base);
}

.contacts-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
  margin-bottom: var(--spacing-lg);
}

.contact-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-sm);
  border-radius: var(--border-radius-md);
  background: var(--color-backplate);
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
}

.contact-item:hover {
  background: var(--color-surface);
  transform: translateX(var(--spacing-xs));
}

.contact-icon {
  font-size: var(--font-size-base);
  width: 24px;
  text-align: center;
}

.contact-link {
  color: var(--color-primary);
  text-decoration: none;
  font-size: var(--font-size-sm);
  transition: color var(--animation-duration-standard) var(--animation-curve-primary);
}

.contact-link:hover {
  color: var(--color-secondary);
}

.skills-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
  margin-bottom: var(--spacing-lg);
}

.skill-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-sm);
  border-radius: var(--border-radius-md);
  background: var(--color-backplate);
}

.skill-info {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.skill-icon {
  font-size: var(--font-size-base);
}

.skill-name {
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
  font-size: var(--font-size-sm);
}

.skill-level {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.level-dots {
  display: flex;
  gap: 2px;
}

.dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: var(--color-border);
  transition: background var(--animation-duration-standard) var(--animation-curve-primary);
}

.dot.active {
  background: var(--color-primary);
}

.level-text {
  font-size: var(--font-size-xs);
  color: var(--color-on-surface-secondary);
  min-width: 60px;
  text-align: right;
}

.full-width {
  width: 100%;
}

/* Основное содержимое профиля */
.profile-main {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

/* Вкладки */
.profile-tabs {
  padding: var(--spacing-md);
}

.tabs-navigation {
  display: flex;
  gap: var(--spacing-xs);
  background: var(--color-backplate);
  padding: var(--spacing-xs);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
}

.tab-btn {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--spacing-sm);
  padding: var(--spacing-md) var(--spacing-lg);
  border: none;
  background: transparent;
  border-radius: var(--border-radius-sm);
  cursor: pointer;
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
  font-size: var(--font-size-base);
  color: var(--color-on-surface-secondary);
  font-weight: var(--font-weight-heading);
  position: relative;
}

.tab-btn:hover {
  color: var(--color-primary);
  background: var(--color-surface);
}

.tab-btn.active {
  background: var(--color-primary);
  color: var(--color-on-primary);
}

.tab-icon {
  font-size: var(--font-size-base);
}

.tab-badge {
  position: absolute;
  top: -4px;
  right: -4px;
  background: var(--color-accent);
  color: var(--color-on-surface);
  font-size: var(--font-size-xs);
  padding: 2px 6px;
  border-radius: var(--border-radius-full);
  font-weight: var(--font-weight-heading);
  min-width: 20px;
  text-align: center;
}

/* Содержимое вкладок */
.tab-content {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.tab-pane {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

/* Карточки контента */
.bio-card,
.activity-card,
.achievements-card,
.stats-overview,
.difficulty-stats,
.activity-chart,
.settings-card {
  padding: var(--spacing-lg);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--spacing-lg);
}

.bio-content {
  line-height: var(--line-height-body);
}

.bio-text {
  margin: 0;
  color: var(--color-on-surface);
  font-size: var(--font-size-base);
  line-height: var(--line-height-body);
}

.bio-empty {
  text-align: center;
  padding: var(--spacing-2xl);
  color: var(--color-on-surface-secondary);
}

.empty-icon {
  font-size: var(--font-size-hero);
  margin-bottom: var(--spacing-lg);
  display: block;
}

.empty-hint {
  font-size: var(--font-size-sm);
  margin-top: var(--spacing-sm);
}

/* Списки активности */
.activity-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.activity-item {
  display: flex;
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  border-radius: var(--border-radius-md);
  background: var(--color-backplate);
  border: 1px solid var(--color-border);
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
}

.activity-item:hover {
  transform: translateX(var(--spacing-xs));
  border-color: var(--color-primary);
}

.activity-icon {
  font-size: var(--font-size-lg);
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-surface);
  border-radius: var(--border-radius-md);
  flex-shrink: 0;
}

.activity-content {
  flex: 1;
}

.activity-text {
  margin: 0 0 var(--spacing-xs) 0;
  color: var(--color-on-surface);
  font-size: var(--font-size-base);
}

.activity-time {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
}

/* Достижения */
.achievements-count {
  color: var(--color-on-surface-secondary);
  font-weight: normal;
  font-size: var(--font-size-base);
}

.achievements-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: var(--spacing-md);
}

.achievement-item {
  display: flex;
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  border-radius: var(--border-radius-md);
  background: var(--color-backplate);
  border: 1px solid var(--color-border);
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
}

.achievement-item:hover {
  transform: translateY(-2px);
  border-color: var(--color-primary);
}

.achievement-icon {
  font-size: var(--font-size-xl);
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-surface);
  border-radius: var(--border-radius-md);
  flex-shrink: 0;
}

.achievement-info {
  flex: 1;
}

.achievement-name {
  margin: 0 0 var(--spacing-xs) 0;
  font-size: var(--font-size-base);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.achievement-description {
  margin: 0 0 var(--spacing-xs) 0;
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  line-height: var(--line-height-body);
}

.achievement-date {
  font-size: var(--font-size-xs);
  color: var(--color-on-surface-secondary);
}

.achievements-empty {
  text-align: center;
  padding: var(--spacing-2xl);
}

/* Статистика */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--spacing-lg);
}

.stat-card {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-lg);
  background: var(--color-backplate);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
  text-align: left;
}

.stat-card .stat-icon {
  font-size: var(--font-size-xl);
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-surface);
  border-radius: var(--border-radius-md);
}

.stat-card .stat-data {
  display: flex;
  flex-direction: column;
}

.stat-card .stat-value {
  font-size: var(--font-size-xl);
  font-weight: var(--font-weight-heading);
  color: var(--color-primary);
  margin-bottom: var(--spacing-xs);
  font-family: var(--font-family-heading);
}

.stat-card .stat-label {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  text-transform: none;
  letter-spacing: normal;
}

/* Статистика по сложности */
.difficulty-grid {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.difficulty-item {
  padding: var(--spacing-md);
  background: var(--color-backplate);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
}

.diff-header {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  margin-bottom: var(--spacing-md);
}

.diff-icon {
  font-size: var(--font-size-base);
}

.diff-name {
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
  font-size: var(--font-size-base);
}

.diff-progress {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.progress-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: var(--font-size-sm);
}

.progress-value {
  color: var(--color-on-surface);
}

.progress-percentage {
  color: var(--color-primary);
  font-weight: var(--font-weight-heading);
}

.progress-fill.easy { background: #10B981; }
.progress-fill.medium { background: #3B82F6; }
.progress-fill.hard { background: #EF4444; }

/* Календарь активности */
.chart-container {
  margin-bottom: var(--spacing-lg);
}

.activity-calendar {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 2px;
  max-width: 300px;
}

.calendar-day {
  width: 100%;
  aspect-ratio: 1;
  background: var(--color-border);
  border-radius: 2px;
  transition: background var(--animation-duration-standard) var(--animation-curve-primary);
}

.calendar-day.none { background: var(--color-border); }
.calendar-day.low { background: color-mix(in srgb, var(--color-primary) 30%, transparent); }
.calendar-day.medium { background: color-mix(in srgb, var(--color-primary) 60%, transparent); }
.calendar-day.high { background: var(--color-primary); }

.chart-legend {
  display: flex;
  gap: var(--spacing-lg);
  justify-content: center;
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
}

.legend-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
}

.legend-color {
  width: 12px;
  height: 12px;
  border-radius: 2px;
}

.legend-color.less { background: var(--color-border); }
.legend-color.more { background: var(--color-primary); }

/* Решенные задачи */
.solved-tasks-header {
  padding: var(--spacing-lg);
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.tasks-count {
  color: var(--color-on-surface-secondary);
  font-weight: normal;
  font-size: var(--font-size-base);
}

.tasks-filters {
  display: flex;
  gap: var(--spacing-md);
}

.solved-tasks-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.task-item {
  padding: var(--spacing-lg);
}

.task-main {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--spacing-md);
}

.task-info {
  flex: 1;
}

.task-title {
  margin: 0 0 var(--spacing-sm) 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.task-meta {
  display: flex;
  gap: var(--spacing-lg);
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
}

.task-difficulty.easy { color: #10B981; }
.task-difficulty.medium { color: #3B82F6; }
.task-difficulty.hard { color: #EF4444; }

.task-language,
.task-time {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
}

.lang-icon,
.time-icon {
  font-size: var(--font-size-base);
}

.task-actions {
  display: flex;
  gap: var(--spacing-sm);
  align-self: center;
}

.task-stats {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  background: var(--color-backplate);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
}

.stat {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: var(--font-size-sm);
}

.stat-label {
  color: var(--color-on-surface-secondary);
}

.stat-value {
  color: var(--color-on-surface);
  font-weight: var(--font-weight-heading);
}

/* Настройки */
.settings-sections {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-2xl);
}

.settings-section {
  padding-bottom: var(--spacing-lg);
  border-bottom: 1px solid var(--color-border);
}

.settings-section:last-child {
  border-bottom: none;
}

.section-title {
  margin: 0 0 var(--spacing-lg) 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

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

.settings-actions {
  display: flex;
  gap: var(--spacing-md);
  justify-content: flex-end;
  margin-top: var(--spacing-lg);
  padding-top: var(--spacing-lg);
  border-top: 1px solid var(--color-border);
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
  .profile-layout {
    grid-template-columns: 300px 1fr;
    gap: var(--spacing-lg);
  }
}

@media (max-width: 1024px) {
  .profile-layout {
    grid-template-columns: 1fr;
  }

  .profile-sidebar {
    position: static;
  }

  .tabs-navigation {
    flex-wrap: wrap;
  }

  .tab-btn {
    flex: 1 1 calc(50% - var(--spacing-xs));
  }
}

@media (max-width: 768px) {
  .container {
    padding: 0 var(--spacing-md);
  }

  .profile-header {
    text-align: left;
    align-items: flex-start;
  }

  .avatar-section {
    display: flex;
    align-items: flex-end;
    gap: var(--spacing-lg);
  }

  .avatar-container {
    margin-bottom: 0;
  }

  .task-main {
    flex-direction: column;
    gap: var(--spacing-md);
  }

  .task-actions {
    align-self: flex-start;
  }

  .task-stats {
    grid-template-columns: 1fr;
  }

  .settings-actions {
    flex-direction: column;
  }

  .stats-grid {
    grid-template-columns: 1fr;
  }

  .achievements-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 480px) {
  .profile-badges {
    flex-direction: row;
    flex-wrap: wrap;
  }

  .task-meta {
    flex-direction: column;
    gap: var(--spacing-sm);
  }

  .tabs-navigation {
    flex-direction: column;
  }

  .tab-btn {
    flex: 1;
  }

  .header-content {
    flex-direction: column;
    gap: var(--spacing-md);
    align-items: flex-start;
  }

  .tasks-filters {
    width: 100%;
    flex-direction: column;
  }

  .setting-item {
    flex-direction: column;
    gap: var(--spacing-sm);
    align-items: flex-start;
  }

  .setting-control {
    align-self: flex-start;
  }
}
</style>