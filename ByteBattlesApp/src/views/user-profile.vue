<template>
  <div class="user-profile-container">
    <app-navigation></app-navigation>

    <div class="user-profile-wrapper">
      <DangerousHTML
          html="<style>
  .user-profile-container {
    min-height: 100vh;
    background: var(--color-surface);
    padding: var(--spacing-2xl) 0;
  }

  .user-profile-container::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-image:
      radial-gradient(circle at 40% 60%, color-mix(in srgb, var(--color-primary) 3%, transparent) 0%, transparent 50%),
      repeating-linear-gradient(
        135deg,
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

  @keyframes slideInUp {
    from {
      opacity: 0;
      transform: translateY(30px);
    }
    to {
      opacity: 1;
      transform: translateY(0);
    }
  }

  .profile-section {
    animation: slideInUp 0.6s var(--animation-curve-primary);
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

      <section class="user-profile-section" role="main" aria-label="Личный кабинет">
        <div class="container">
          <!-- Хлебные крошки -->
          <nav class="breadcrumbs" aria-label="Навигация">
            <ol class="breadcrumbs-list">
              <li class="breadcrumb-item">
                <router-link to="/" class="breadcrumb-link">Главная</router-link>
              </li>
              <li class="breadcrumb-separator">/</li>
              <li class="breadcrumb-item">
                <span class="breadcrumb-current">Личный кабинет</span>
              </li>
            </ol>
          </nav>

          <div class="page-header">
            <div class="title-section">
              <h1 class="page-title">
                Личный кабинет преподавателя
              </h1>
              <p class="page-subtitle">
                Управляйте вашим профилем, настройками и созданными задачами
              </p>
            </div>
          </div>

          <div class="profile-layout">
            <!-- Боковая панель -->
            <aside class="profile-sidebar" role="complementary" aria-label="Навигация по кабинету">
              <!-- Профиль пользователя -->
              <div class="user-card retro-card">
                <div class="user-header">
                  <div class="avatar-section">
                    <div class="avatar-container">
                      <img
                          :src="userData.avatar"
                          :alt="userData.fullName"
                          class="profile-avatar"
                          v-if="userData.avatar"
                      >
                      <div class="avatar-placeholder" v-else>
                        {{ userData.fullName.charAt(0).toUpperCase() }}
                      </div>
                      <div class="online-status" :class="{ online: userData.isOnline }">
                        {{ userData.isOnline ? 'Онлайн' : 'Не в сети' }}
                      </div>
                    </div>
                    <button @click="openAvatarEditor" class="btn-text btn-sm avatar-edit-btn">
                      Сменить фото
                    </button>
                  </div>

                  <div class="user-basic-info">
                    <h2 class="user-name">{{ userData.fullName }}</h2>
                    <p class="user-role">Преподаватель</p>
                    <p class="user-email">{{ userData.email }}</p>

                    <div class="user-stats">
                      <div class="stat">
                        <span class="stat-value">{{ userStats.createdTasks }}</span>
                        <span class="stat-label">создано задач</span>
                      </div>
                      <div class="stat">
                        <span class="stat-value">{{ userStats.activeStudents }}</span>
                        <span class="stat-label">студентов</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Навигация -->
              <div class="profile-nav retro-card">
                <h3 class="nav-title">
                  Навигация
                </h3>
                <nav class="nav-list">
                  <button
                      v-for="item in navItems"
                      :key="item.id"
                      :class="['nav-item', { 'active': activeSection === item.id }]"
                      @click="activeSection = item.id"
                  >

                    <span class="nav-item-text">{{ item.name }}</span>
                    <span class="nav-item-badge" v-if="item.badge">{{ item.badge }}</span>
                  </button>
                </nav>
              </div>

              <!-- Быстрые действия -->
              <div class="quick-actions retro-card">
                <h3 class="actions-title">
                  Быстрые действия
                </h3>
                <div class="actions-list">
                  <router-link to="/task-template-builder" class="btn-primary full-width">
                    Создать задачу
                  </router-link>
                  <router-link to="/tasks" class="btn-outline full-width">
                    Мои задачи
                  </router-link>
                </div>
              </div>
            </aside>

            <!-- Основное содержимое -->
            <main class="profile-main" role="region" :aria-label="`Раздел: ${getActiveSectionName()}`">
              <!-- Раздел: Личная информация -->
              <div v-if="activeSection === 'personal'" class="profile-section">
                <div class="section-header">
                  <h2>
                    Личная информация
                  </h2>
                  <p>Управляйте вашими персональными данными и настройками профиля</p>
                </div>

                <div class="form-grid">
                  <div class="form-section retro-card">
                    <h3>Основные данные</h3>

                    <div class="form-group">
                      <label for="full-name" class="required">
                        Полное имя
                      </label>
                      <div class="input-container vintage-border">
                        <input
                            type="text"
                            id="full-name"
                            v-model="userData.fullName"
                            placeholder="Введите ваше полное имя"
                            :class="{ 'error': errors.fullName }"
                        >
                      </div>
                      <div class="error-message" v-if="errors.fullName">{{ errors.fullName }}</div>
                    </div>

                    <div class="form-group">
                      <label for="bio">
                        О себе
                      </label>
                      <div class="input-container vintage-border">
                        <textarea
                            id="bio"
                            v-model="userData.bio"
                            rows="4"
                            placeholder="Расскажите о себе, ваших интересах и опыте..."
                            maxlength="500"
                        ></textarea>
                      </div>
                      <div class="char-counter">{{ userData.bio.length }}/500</div>
                    </div>
                  </div>

                  <div class="form-section retro-card">
                    <h3>Контактная информация</h3>

                    <div class="form-group">
                      <label for="email" class="required">
                        Email адрес
                      </label>
                      <div class="input-container vintage-border">
                        <input
                            type="email"
                            id="email"
                            v-model="userData.email"
                            placeholder="your.email@example.com"
                            :class="{ 'error': errors.email }"
                        >
                      </div>
                      <div class="error-message" v-if="errors.email">{{ errors.email }}</div>
                    </div>



                    <div class="form-group">
                      <label>Социальные сети</label>
                      <div class="social-links">
                        <div class="social-input">

                          <input
                              type="url"
                              v-model="userData.linkedInUrl"
                              placeholder="LinkedIn профиль"
                              class="vintage-border"
                          >
                        </div>
                        <div class="social-input">
                          <input
                              type="url"
                              v-model="userData.gitHubUrl"
                              placeholder="GitHub профиль"
                              class="vintage-border"
                          >
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <div class="form-actions">
                  <button @click="savePersonalInfo" class="btn-primary" :disabled="isSaving">
                    {{ isSaving ? 'Сохранение...' : 'Сохранить изменения' }}
                  </button>
                  <button @click="resetPersonalInfo" class="btn-outline">
                    Отменить изменения
                  </button>
                </div>
              </div>

              <!-- Раздел: Безопасность -->
              <div v-if="activeSection === 'security'" class="profile-section">
                <div class="section-header">
                  <h2>
                    <span class="section-icon">🔒</span>
                    Безопасность и пароль
                  </h2>
                  <p>Обновите ваш пароль и настройки безопасности аккаунта</p>
                </div>

                <div class="form-grid">
                  <div class="form-section retro-card">
                    <h3>Смена пароля</h3>

                    <div class="form-group">
                      <label for="current-password" class="required">
                        Текущий пароль
                      </label>
                      <div class="input-container vintage-border">
                        <input
                            type="password"
                            id="current-password"
                            v-model="passwordData.currentPassword"
                            placeholder="Введите текущий пароль"
                            :class="{ 'error': errors.currentPassword }"
                        >
                        <button
                            @click="togglePasswordVisibility('current')"
                            class="password-toggle"
                            type="button"
                        >
                          <span class="toggle-icon">{{ showPasswords.current ? '👁️' : '👁️‍🗨️' }}</span>
                        </button>
                      </div>
                      <div class="error-message" v-if="errors.currentPassword">{{ errors.currentPassword }}</div>
                    </div>

                    <div class="form-group">
                      <label for="new-password" class="required">
                        Новый пароль
                      </label>
                      <div class="input-container vintage-border">
                        <input
                            :type="showPasswords.new ? 'text' : 'password'"
                            id="new-password"
                            v-model="passwordData.newPassword"
                            placeholder="Введите новый пароль"
                            :class="{ 'error': errors.newPassword }"
                        >
                        <button
                            @click="togglePasswordVisibility('new')"
                            class="password-toggle"
                            type="button"
                        >
                          <span class="toggle-icon">{{ showPasswords.new ? '👁️' : '👁️‍🗨️' }}</span>
                        </button>
                      </div>
                      <div class="password-strength" :class="passwordStrength">
                        <div class="strength-bar">
                          <div class="strength-fill" :style="{ width: passwordStrengthPercent + '%' }"></div>
                        </div>
                        <span class="strength-text">{{ passwordStrengthText }}</span>
                      </div>
                      <div class="error-message" v-if="errors.newPassword">{{ errors.newPassword }}</div>
                    </div>

                    <div class="form-group">
                      <label for="confirm-password" class="required">
                        Подтверждение пароля
                      </label>
                      <div class="input-container vintage-border">
                        <input
                            :type="showPasswords.confirm ? 'text' : 'password'"
                            id="confirm-password"
                            v-model="passwordData.confirmPassword"
                            placeholder="Повторите новый пароль"
                            :class="{ 'error': errors.confirmPassword }"
                        >
                        <button
                            @click="togglePasswordVisibility('confirm')"
                            class="password-toggle"
                            type="button"
                        >
                          <span class="toggle-icon">{{ showPasswords.confirm ? '👁️' : '👁️‍🗨️' }}</span>
                        </button>
                      </div>
                      <div class="error-message" v-if="errors.confirmPassword">{{ errors.confirmPassword }}</div>
                    </div>

                    <div class="password-requirements">
                      <h4>Требования к паролю:</h4>
                      <ul>
                        <li :class="{ 'met': passwordData.newPassword.length >= 8 }">
                          Минимум 8 символов
                        </li>
                        <li :class="{ 'met': /[A-Z]/.test(passwordData.newPassword) }">
                          Хотя бы одна заглавная буква
                        </li>
                        <li :class="{ 'met': /[a-z]/.test(passwordData.newPassword) }">
                          Хотя бы одна строчная буква
                        </li>
                        <li :class="{ 'met': /[0-9]/.test(passwordData.newPassword) }">
                          Хотя бы одна цифра
                        </li>
                        <li :class="{ 'met': /[^A-Za-z0-9]/.test(passwordData.newPassword) }">
                          Хотя бы один специальный символ
                        </li>
                      </ul>
                    </div>
                  </div>
                </div>

                <div class="form-actions">
                  <button @click="updatePassword" class="btn-primary" :disabled="!canChangePassword || isSaving">
                    <span class="btn-icon">🔒</span>
                    {{ isSaving ? 'Обновление...' : 'Обновить пароль' }}
                  </button>
                  <button @click="resetPasswordForm" class="btn-outline">
                    <span class="btn-icon">🔄</span>
                    Очистить форму
                  </button>
                </div>
              </div>



              <!-- Раздел: Статистика -->
              <div v-if="activeSection === 'stats'" class="profile-section">
                <div class="section-header">
                  <h2>
                    Статистика и активность
                  </h2>
                  <p>Просмотр вашей активности и статистики созданных задач</p>
                </div>

                <div class="stats-overview">
                  <div class="stats-grid">
                    <div class="stat-card retro-card">
                      <div class="stat-data">
                        <span class="stat-value">{{ userStats.createdTasks }}</span>
                        <span class="stat-label">Создано задач</span>
                      </div>
                    </div>
                    <div class="stat-card retro-card">
                      <div class="stat-data">
                        <span class="stat-value">{{ userStats.solvedTasks }}</span>
                        <span class="stat-label">Решено студентами</span>
                      </div>
                    </div>
                    <div class="stat-card retro-card">
                      <div class="stat-data">
                        <span class="stat-value">{{ userStats.activeStudents }}</span>
                        <span class="stat-label">Активных студентов</span>
                      </div>
                    </div>
                  </div>
                </div>

                <div class="activity-charts">
                  <div class="chart-section retro-card">
                    <h3>Активность за последние 30 дней</h3>
                    <div class="chart-container">
                      <div class="activity-bars">
                        <div
                            v-for="day in activityData"
                            :key="day.date"
                            class="activity-bar"
                            :title="`${day.date}: ${day.tasksCreated} задач создано`"
                        >
                          <div
                              class="bar-fill"
                              :style="{ height: day.tasksCreated * 2 + 'px' }"
                              :class="getActivityLevel(day.tasksCreated)"
                          ></div>
                          <span class="bar-label">{{ day.date.split('-')[2] }}</span>
                        </div>
                      </div>
                    </div>
                  </div>

                  <div class="tasks-breakdown retro-card">
                    <h3>Распределение задач по сложности</h3>
                    <div class="breakdown-chart">
                      <div
                          v-for="diff in difficultyBreakdown"
                          :key="diff.level"
                          class="breakdown-item"
                      >
                        <div class="diff-info">
                          <span class="diff-icon">{{ diff.icon }}</span>
                          <span class="diff-label">{{ diff.label }}</span>
                        </div>
                        <div class="diff-stats">
                          <span class="diff-count">{{ diff.count }}</span>
                          <div class="diff-bar">
                            <div
                                class="diff-fill"
                                :style="{ width: diff.percentage + '%' }"
                                :class="diff.level"
                            ></div>
                          </div>
                          <span class="diff-percentage">{{ diff.percentage }}%</span>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <div class="recent-activity retro-card">
                  <h3>Последняя активность</h3>
                  <div class="activity-list">
                    <div
                        v-for="activity in recentActivities"
                        :key="activity.id"
                        class="activity-item"
                    >

                      <div class="activity-content">
                        <p class="activity-text">{{ activity.text }}</p>
                        <span class="activity-time">{{ activity.time }}</span>
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

    <!-- Редактор аватара -->
    <div v-if="showAvatarEditor" class="dialog-overlay">
      <div class="dialog retro-card">
        <h3 class="dialog-title">Смена аватара</h3>
        <div class="avatar-editor">
          <div class="avatar-preview">
            <img
                :src="avatarPreview"
                :alt="userData.fullName"
                class="preview-avatar"
                v-if="avatarPreview"
            >
            <div class="avatar-placeholder-large" v-else>
              {{ userData.fullName.charAt(0).toUpperCase() }}
            </div>
          </div>
          <div class="avatar-options">
            <input
                type="file"
                ref="avatarInput"
                @change="handleAvatarUpload"
                accept="image/*"
                hidden
            >
            <button @click="$refs.avatarInput.click()" class="btn-outline full-width">
              <span class="btn-icon">📁</span>
              Выбрать файл
            </button>
            <button @click="generateAvatar" class="btn-text full-width">
              <span class="btn-icon">🎨</span>
              Сгенерировать аватар
            </button>
            <button @click="removeAvatar" class="btn-text full-width delete-btn">
              <span class="btn-icon">🗑️</span>
              Удалить аватар
            </button>
          </div>
        </div>
        <div class="dialog-actions">
          <button @click="showAvatarEditor = false" class="btn-outline">Отмена</button>
          <button @click="saveAvatar" class="btn-primary">Сохранить</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import DangerousHTML from 'dangerous-html/vue'
import AppNavigation from '../components/navigation'
import AppFooter from '../components/footer'
import {userProfilesAPI} from '../api/user'
import {taskAPI} from "@/api/task";

export default {
  name: 'UserProfile',
  components: {
    AppNavigation,
    DangerousHTML,
    AppFooter,
  },
  data() {
    return {
      activeSection: 'personal',
      isSaving: false,
      isLoading: true,
      showAvatarEditor: false,
      showPasswords: {
        current: false,
        new: false,
        confirm: false
      },

      navItems: [
        { id: 'personal', name: 'Личная информация', badge: null },
        { id: 'security', name: 'Безопасность', badge: null },
        { id: 'stats', name: 'Статистика', badge: null }
      ],

      // Данные из API
      userData: {
        fullName: '',
        email: '',
        phone: '',
        country: '',
        bio: '',
        avatar: '',
        isOnline: false,
        gitHubUrl: '',
        linkedInUrl: ''

      },
      countMedium :0,
      countHard :0,
      countEasy: 0,
      userStats: {
        createdTasks: 0,
        solvedTasks: 0,
        averageRating: 0,
        activeStudents: 0
      },

      passwordData: {
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
      },

      activityData: [],
      difficultyBreakdown: [],
      recentActivities: [],

      errors: {},
      avatarPreview: null,
      originalUserData: null,
      selectedAvatarFile: null
    }
  },
  computed: {
    // ... остальные computed свойства без изменений
    passwordStrength() {
      if (!this.passwordData.newPassword) return 'empty'
      const strength = this.calculatePasswordStrength(this.passwordData.newPassword)
      if (strength < 40) return 'weak'
      if (strength < 70) return 'medium'
      return 'strong'
    },

    passwordStrengthPercent() {
      return this.calculatePasswordStrength(this.passwordData.newPassword)
    },

    passwordStrengthText() {
      const texts = {
        empty: 'Введите пароль',
        weak: 'Слабый',
        medium: 'Средний',
        strong: 'Сильный'
      }
      return texts[this.passwordStrength]
    },

    canChangePassword() {
      return this.passwordData.currentPassword &&
          this.passwordData.newPassword &&
          this.passwordData.confirmPassword &&
          this.passwordData.newPassword === this.passwordData.confirmPassword &&
          this.passwordStrength !== 'weak'
    }
  },
  async mounted() {
    await this.loadUserProfile()
  },
  methods: {
    // Загрузка профиля пользователя
    async loadUserProfile() {
      this.isLoading = true
      try {
        const profileData = await userProfilesAPI.getMyProfile()
        this.mapApiDataToUserProfile(profileData)
        this.originalUserData = JSON.parse(JSON.stringify(this.userData))
      } catch (error) {
        console.error('Ошибка загрузки профиля:', error)
        this.$notify({
          type: 'error',
          title: 'Ошибка',
          text: 'Не удалось загрузить данные профиля'
        })
      } finally {
        this.isLoading = false
      }
    },


    // Преобразование данных из API в формат компонента
// Преобразование данных из API в формат компонента
    mapApiDataToUserProfile(apiData) {
      console.log('Данные из API:', apiData)

      // Основные поля профиля
      this.userData = {
        fullName: apiData.userName || 'Пользователь',
        email: apiData.email || 'email@example.com',
        phone: apiData.phone || '',
        country: apiData.country || '',
        bio: apiData.bio || '',
        avatar: apiData.avatarUrl || '', // Обратите внимание: avatarUrl в API
        linkedInUrl: apiData.linkedInUrl || '',
        gitHubUrl: apiData.gitHubUrl || '',
        isOnline: true
      }

      // Статистика (если есть в API)
      if (apiData.teacherStats) {
        this.userStats = {
          createdTasks: apiData.stats.createdTasks || 0,
          solvedTasks: apiData.stats.totalProblemsSolved || 0, // Обратите внимание на название поля
          averageRating: 0,
          activeStudents: 0
        }
      } else {
        // Заглушка для демонстрации
        this.userStats = {
          createdTasks: 24,
          solvedTasks: apiData.stats?.totalProblemsSolved || 1567,
          averageRating: 4.7,
          activeStudents: 89
        }
      }

      // Генерируем тестовые данные для демонстрации
      this.generateMockData()
    },

    // Преобразование данных для отправки в API
    prepareProfileForApi() {
      return {
        userName: this.userData.fullName,
        bio: this.userData.bio,
        linkedInUrl: this.userData.linkedInUrl,
        gitHubUrl: this.userData.gitHubUrl,
      }
    },

    // Сохранение личной информации
    async savePersonalInfo() {
      this.validatePersonalInfo()
      if (Object.keys(this.errors).length > 0) return

      this.isSaving = true
      try {
        const updateData = this.prepareProfileForApi()
        await userProfilesAPI.updateMyProfile(updateData)

        this.originalUserData = JSON.parse(JSON.stringify(this.userData))

        this.$notify({
          type: 'success',
          title: 'Успешно',
          text: 'Данные профиля сохранены'
        })
      } catch (error) {
        console.error('Ошибка сохранения:', error)
        this.$notify({
          type: 'error',
          title: 'Ошибка',
          text: 'Не удалось сохранить данные профиля'
        })
      } finally {
        this.isSaving = false
      }
    },

    // Смена пароля
    async updatePassword() {
      this.validatePassword()
      if (Object.keys(this.errors).length > 0) return

      this.isSaving = true
      try {
        await userProfilesAPI.changePassword({
          oldPassword: this.passwordData.currentPassword,
          newPassword: this.passwordData.newPassword
        })

        this.$notify({
          type: 'success',
          title: 'Успешно',
          text: 'Пароль успешно изменен'
        })

        this.resetPasswordForm()
      } catch (error) {
        console.error('Ошибка смены пароля:', error)
        this.$notify({
          type: 'error',
          title: 'Ошибка',
          text: error.message || 'Не удалось изменить пароль'
        })
      } finally {
        this.isSaving = false
      }
    },

    // Загрузка аватара
    async saveAvatar() {
      if (!this.selectedAvatarFile && !this.avatarPreview) {
        this.showAvatarEditor = false
        return
      }

      this.isSaving = true
      try {
        if (this.selectedAvatarFile) {
          // Если выбран файл - загружаем его
          await userProfilesAPI.uploadAvatar(this.selectedAvatarFile)
        } else if (this.avatarPreview && this.avatarPreview.startsWith('data:image/svg+xml')) {
          // Если сгенерирован SVG аватар - обрабатываем особым образом
          await this.saveGeneratedAvatar()
        }

        // Обновляем данные профиля
        await this.loadUserProfile()

        this.showAvatarEditor = false
        this.selectedAvatarFile = null

        this.$notify({
          type: 'success',
          title: 'Успешно',
          text: 'Аватар обновлен'
        })
      } catch (error) {
        console.error('Ошибка сохранения аватара:', error)
        this.$notify({
          type: 'error',
          title: 'Ошибка',
          text: 'Не удалось сохранить аватар'
        })
      } finally {
        this.isSaving = false
      }
    },

    // Обработка загрузки файла аватара
    handleAvatarUpload(event) {
      const file = event.target.files[0]
      if (file) {
        // Проверяем тип и размер файла
        if (!file.type.startsWith('image/')) {
          this.$notify({
            type: 'error',
            title: 'Ошибка',
            text: 'Пожалуйста, выберите файл изображения'
          })
          return
        }

        if (file.size > 5 * 1024 * 1024) { // 5MB
          this.$notify({
            type: 'error',
            title: 'Ошибка',
            text: 'Размер файла не должен превышать 5MB'
          })
          return
        }

        this.selectedAvatarFile = file

        const reader = new FileReader()
        reader.onload = (e) => {
          this.avatarPreview = e.target.result
        }
        reader.readAsDataURL(file)
      }
    },

    // Сохранение сгенерированного аватара
    async saveGeneratedAvatar() {
      // Конвертируем SVG в Blob и отправляем
      const svgContent = atob(this.avatarPreview.split(',')[1])
      const blob = new Blob([svgContent], { type: 'image/svg+xml' })
      const file = new File([blob], 'avatar.svg', { type: 'image/svg+xml' })


    },

    // Удаление аватара
    async removeAvatar() {
      try {
        // Если есть эндпоинт для удаления аватара
        await makeRequest('/api/user-profiles/me/avatar', {
          method: 'DELETE'
        })

        this.avatarPreview = null
        this.selectedAvatarFile = null
        await this.loadUserProfile()
      } catch (error) {
        console.error('Ошибка удаления аватара:', error)
      }
    },

    // Вспомогательные методы
    getActivityIcon(activityType) {
      const icons = {
        TASK_CREATED: '📝',
        TASK_SOLVED: '✅',
        REVIEW_RECEIVED: '⭐',
        TASK_UPDATED: '🔧'
      }
      return icons[activityType] || '📌'
    },

    formatTime(timestamp) {
      const now = new Date()
      const activityDate = new Date(timestamp)
      const diffMs = now - activityDate
      const diffMins = Math.floor(diffMs / 60000)
      const diffHours = Math.floor(diffMs / 3600000)
      const diffDays = Math.floor(diffMs / 86400000)

      if (diffMins < 60) {
        return `${diffMins} минут назад`
      } else if (diffHours < 24) {
        return `${diffHours} часов назад`
      } else {
        return `${diffDays} дней назад`
      }
    },

    // Остальные методы без изменений
    getActiveSectionName() {
      const item = this.navItems.find(item => item.id === this.activeSection)
      return item ? item.name : ''
    },

    openAvatarEditor() {
      this.avatarPreview = this.userData.avatar
      this.showAvatarEditor = true
    },

    generateAvatar() {
      const colors = ['#FF6B6B', '#4ECDC4', '#45B7D1', '#96CEB4', '#FFEAA7']
      const color = colors[Math.floor(Math.random() * colors.length)]
      const initials = this.userData.fullName.split(' ').map(n => n[0]).join('')

      const svg = `
        <svg width="200" height="200" viewBox="0 0 200 200" xmlns="http://www.w3.org/2000/svg">
          <rect width="200" height="200" fill="${color}" rx="100"/>
          <text x="100" y="120" text-anchor="middle" fill="white" font-size="80" font-family="Arial, sans-serif" font-weight="bold">
            ${initials}
          </text>
        </svg>
      `

      this.avatarPreview = 'data:image/svg+xml;base64,' + btoa(svg)
      this.selectedAvatarFile = null
    },

    resetPersonalInfo() {
      this.userData = JSON.parse(JSON.stringify(this.originalUserData))
      this.errors = {}
    },

    validatePersonalInfo() {
      this.errors = {}

      if (!this.userData.fullName?.trim()) {
        this.errors.fullName = 'Полное имя обязательно'
      }

      if (!this.userData.email?.trim()) {
        this.errors.email = 'Email обязателен'
      } else if (!this.isValidEmail(this.userData.email)) {
        this.errors.email = 'Введите корректный email'
      }
    },

    validatePassword() {
      this.errors = {}

      if (!this.passwordData.currentPassword) {
        this.errors.currentPassword = 'Введите текущий пароль'
      }

      if (!this.passwordData.newPassword) {
        this.errors.newPassword = 'Введите новый пароль'
      } else if (this.passwordStrength === 'weak') {
        this.errors.newPassword = 'Пароль слишком слабый'
      }

      if (!this.passwordData.confirmPassword) {
        this.errors.confirmPassword = 'Подтвердите пароль'
      } else if (this.passwordData.newPassword !== this.passwordData.confirmPassword) {
        this.errors.confirmPassword = 'Пароли не совпадают'
      }
    },

    resetPasswordForm() {
      this.passwordData = {
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
      }
      this.errors = {}
    },

    togglePasswordVisibility(field) {
      this.showPasswords[field] = !this.showPasswords[field]
    },

    calculatePasswordStrength(password) {
      let strength = 0

      if (password.length >= 8) strength += 25
      if (/[A-Z]/.test(password)) strength += 25
      if (/[a-z]/.test(password)) strength += 25
      if (/[0-9]/.test(password)) strength += 15
      if (/[^A-Za-z0-9]/.test(password)) strength += 10

      return Math.min(strength, 100)
    },

    getActivityLevel(count) {
      if (count === 0) return 'none'
      if (count <= 1) return 'low'
      if (count <= 3) return 'medium'
      return 'high'
    },

    isValidEmail(email) {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
      return emailRegex.test(email)
    }
  }
}
</script>

<style scoped>
/* Стили из предыдущих компонентов + дополнительные для личного кабинета */

.user-profile-container {
  width: 100%;
  display: block;
  min-height: 100vh;
  font-family: var(--font-family-body);
  background: var(--color-surface);
  position: relative;
}

.user-profile-wrapper {
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

/* Основной лейаут */
.profile-layout {
  display: grid;
  grid-template-columns: 320px 1fr;
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

/* Карточка пользователя */
.user-card {
  padding: var(--spacing-lg);
}

.user-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
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

.user-basic-info {
  width: 100%;
}

.user-name {
  margin: 0 0 var(--spacing-xs) 0;
  font-size: var(--font-size-xl);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.user-role {
  margin: 0 0 var(--spacing-xs) 0;
  color: var(--color-primary);
  font-weight: var(--font-weight-heading);
  font-size: var(--font-size-sm);
}

.user-email {
  margin: 0 0 var(--spacing-lg) 0;
  color: var(--color-on-surface-secondary);
  font-size: var(--font-size-sm);
}

.user-stats {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  background: var(--color-backplate);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
}

.stat {
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
  font-size: var(--font-size-xs);
  color: var(--color-on-surface-secondary);
  text-transform: lowercase;
}

/* Навигация */
.profile-nav {
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

/* Быстрые действия */
.quick-actions {
  padding: var(--spacing-lg);
}

.actions-title {
  margin: 0 0 var(--spacing-lg) 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.actions-icon {
  font-size: var(--font-size-base);
}

.actions-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.full-width {
  width: 100%;
}

/* Основное содержимое */
.profile-main {
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

/* Формы */
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
}

.form-group {
  margin-bottom: var(--spacing-lg);
}

.form-group label {
  display: block;
  margin-bottom: var(--spacing-sm);
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
}

.form-group label.required::after {
  content: " *";
  color: #EF4444;
}

.input-container {
  position: relative;
  padding: var(--spacing-xs);
}

.vintage-border {
  border: 1px solid var(--color-border);
  border-radius: var(--border-radius-md);
  background: var(--color-surface);
  box-shadow:
      inset 0 1px 2px color-mix(in srgb, var(--color-on-surface) 3%, transparent),
      0 2px 4px color-mix(in srgb, var(--color-neutral) 5%, transparent);
}

.form-group input,
.form-group textarea,
.form-group select {
  width: 100%;
  padding: var(--spacing-md);
  border: none;
  border-radius: var(--border-radius-sm);
  font-size: var(--font-size-base);
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
  min-height: 100px;
}

.form-group input.error,
.form-group textarea.error {
  background: color-mix(in srgb, #EF4444 5%, transparent);
}

.char-counter {
  text-align: right;
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  margin-top: var(--spacing-xs);
}

.error-message {
  color: #EF4444;
  font-size: var(--font-size-sm);
  margin-top: var(--spacing-xs);
  font-weight: var(--font-weight-heading);
}

/* Социальные сети */
.social-links {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.social-input {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.social-icon {
  font-size: var(--font-size-base);
  width: 24px;
  text-align: center;
}

/* Пароль */
.password-toggle {
  position: absolute;
  right: var(--spacing-md);
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  color: var(--color-on-surface-secondary);
  cursor: pointer;
  padding: var(--spacing-xs);
}

.toggle-icon {
  font-size: var(--font-size-base);
}

.password-strength {
  margin-top: var(--spacing-sm);
}

.strength-bar {
  height: 4px;
  background: var(--color-border);
  border-radius: var(--border-radius-full);
  overflow: hidden;
  margin-bottom: var(--spacing-xs);
}

.strength-fill {
  height: 100%;
  border-radius: var(--border-radius-full);
  transition: width var(--animation-duration-slow) var(--animation-curve-primary);
}

.password-strength.empty .strength-fill {
  background: var(--color-border);
  width: 0%;
}

.password-strength.weak .strength-fill {
  background: #EF4444;
  width: 33%;
}

.password-strength.medium .strength-fill {
  background: #F59E0B;
  width: 66%;
}

.password-strength.strong .strength-fill {
  background: #10B981;
  width: 100%;
}

.strength-text {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
}

.password-requirements {
  margin-top: var(--spacing-lg);
  padding: var(--spacing-md);
  background: var(--color-backplate);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
}

.password-requirements h4 {
  margin: 0 0 var(--spacing-sm) 0;
  font-size: var(--font-size-base);
  color: var(--color-on-surface);
}

.password-requirements ul {
  margin: 0;
  padding-left: var(--spacing-lg);
}

.password-requirements li {
  color: var(--color-on-surface-secondary);
  font-size: var(--font-size-sm);
  margin-bottom: var(--spacing-xs);
  transition: color var(--animation-duration-standard) var(--animation-curve-primary);
}

.password-requirements li.met {
  color: #10B981;
}

/* Настройки безопасности */
.security-settings {
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

.security-actions {
  display: flex;
  gap: var(--spacing-sm);
  margin-top: var(--spacing-lg);
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

/* Уведомления */
.notifications-list,
.channels-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.notification-item,
.channel-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-md);
  background: var(--color-backplate);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
}

.notification-info,
.channel-info {
  flex: 1;
}

.notification-info h4,
.channel-info h4 {
  margin: 0 0 var(--spacing-xs) 0;
  font-size: var(--font-size-base);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.notification-info p,
.channel-info p {
  margin: 0;
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
}

.notification-control,
.channel-control {
  flex-shrink: 0;
}

.frequency-settings {
  margin-top: var(--spacing-lg);
  padding-top: var(--spacing-lg);
  border-top: 1px solid var(--color-border);
}

.frequency-settings h4 {
  margin: 0 0 var(--spacing-md) 0;
  font-size: var(--font-size-base);
  color: var(--color-on-surface);
}

.frequency-options {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(100px, 1fr));
  gap: var(--spacing-sm);
}

.frequency-option {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: var(--spacing-md);
  cursor: pointer;
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
  text-align: center;
}

.frequency-option:hover {
  border-color: var(--color-primary);
}

.frequency-option.selected {
  border-color: var(--color-primary);
  background: color-mix(in srgb, var(--color-primary) 8%, transparent);
}

.freq-icon {
  font-size: var(--font-size-xl);
  margin-bottom: var(--spacing-xs);
}

.freq-label {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-heading);
}

/* Статистика */
.stats-overview {
  margin-bottom: var(--spacing-xl);
}

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
}

.activity-charts {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: var(--spacing-lg);
  margin-bottom: var(--spacing-xl);
}

.chart-section,
.tasks-breakdown {
  padding: var(--spacing-lg);
}

.chart-section h3,
.tasks-breakdown h3 {
  margin: 0 0 var(--spacing-lg) 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.chart-container {
  padding: var(--spacing-md);
}

.activity-bars {
  display: flex;
  align-items: end;
  gap: 2px;
  height: 120px;
  padding: var(--spacing-md) 0;
}

.activity-bar {
  display: flex;
  flex-direction: column;
  align-items: center;
  flex: 1;
  gap: var(--spacing-xs);
}

.bar-fill {
  width: 100%;
  border-radius: 2px 2px 0 0;
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
  min-height: 2px;
}

.bar-fill.none { background: var(--color-border); }
.bar-fill.low { background: color-mix(in srgb, var(--color-primary) 30%, transparent); }
.bar-fill.medium { background: color-mix(in srgb, var(--color-primary) 60%, transparent); }
.bar-fill.high { background: var(--color-primary); }

.bar-label {
  font-size: var(--font-size-xs);
  color: var(--color-on-surface-secondary);
}

.breakdown-chart {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.breakdown-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-md);
  background: var(--color-backplate);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--color-border);
}

.diff-info {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.diff-icon {
  font-size: var(--font-size-base);
}

.diff-label {
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
}

.diff-stats {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
}

.diff-count {
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
  min-width: 20px;
  text-align: center;
}

.diff-bar {
  width: 100px;
  height: 8px;
  background: var(--color-border);
  border-radius: var(--border-radius-full);
  overflow: hidden;
}

.diff-fill {
  height: 100%;
  border-radius: var(--border-radius-full);
  transition: width var(--animation-duration-slow) var(--animation-curve-primary);
}

.diff-fill.easy { background: #10B981; }
.diff-fill.medium { background: #3B82F6; }
.diff-fill.hard { background: #EF4444; }

.diff-percentage {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  min-width: 30px;
  text-align: right;
}

.recent-activity {
  padding: var(--spacing-lg);
}

.recent-activity h3 {
  margin: 0 0 var(--spacing-lg) 0;
  font-size: var(--font-size-lg);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

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

/* Действия форм */
.form-actions {
  display: flex;
  gap: var(--spacing-md);
  justify-content: flex-start;
  padding-top: var(--spacing-lg);
  border-top: 1px solid var(--color-border);
}

/* Диалоги */
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
  max-width: 500px;
  width: 100%;
}

.dialog-title {
  margin: 0 0 var(--spacing-lg) 0;
  font-size: var(--font-size-xl);
  color: var(--color-on-surface);
  font-family: var(--font-family-heading);
}

.avatar-editor {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
  margin-bottom: var(--spacing-xl);
}

.avatar-preview {
  display: flex;
  justify-content: center;
}

.preview-avatar {
  width: 150px;
  height: 150px;
  border-radius: 50%;
  object-fit: cover;
  border: 4px solid var(--color-primary);
}

.avatar-placeholder-large {
  width: 150px;
  height: 150px;
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

.avatar-options {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.dialog-actions {
  display: flex;
  gap: var(--spacing-md);
  justify-content: flex-end;
}

.delete-btn {
  color: #EF4444;
  border-color: #EF4444;
}

.delete-btn:hover:not(:disabled) {
  background: color-mix(in srgb, #EF4444 15%, transparent);
  color: #EF4444;
}

/* Кнопки */
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

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
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
    grid-template-columns: 280px 1fr;
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

  .form-grid {
    grid-template-columns: 1fr;
  }

  .activity-charts {
    grid-template-columns: 1fr;
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

  .user-header {
    flex-direction: row;
    text-align: left;
    gap: var(--spacing-lg);
  }

  .avatar-section {
    margin-bottom: 0;
  }

  .user-stats {
    grid-template-columns: repeat(2, 1fr);
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

  .form-actions {
    flex-direction: column;
  }

  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .activity-bars {
    gap: 1px;
  }

  .dialog-actions {
    flex-direction: column;
  }
}

@media (max-width: 480px) {
  .user-header {
    flex-direction: column;
    text-align: center;
  }

  .stats-grid {
    grid-template-columns: 1fr;
  }

  .setting-item {
    flex-direction: column;
    gap: var(--spacing-md);
    align-items: flex-start;
  }

  .notification-item,
  .channel-item {
    flex-direction: column;
    gap: var(--spacing-md);
    align-items: flex-start;
  }

  .notification-control,
  .channel-control {
    align-self: flex-end;
  }

  .frequency-options {
    grid-template-columns: 1fr;
  }

  .breakdown-item {
    flex-direction: column;
    gap: var(--spacing-md);
    align-items: flex-start;
  }

  .diff-stats {
    width: 100%;
    justify-content: space-between;
  }
}
</style>