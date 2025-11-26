<template>
  <div class="navigation-container1">
    <nav id="mainNavigation" class="navigation">
      <div class="navigation__container">
        <a href="/">
          <div
              aria-label="ByteBattles Template - Главная страница"
              class="navigation__logo"
          >
            <svg
                width="24"
                xmlns="http://www.w3.org/2000/svg"
                height="24"
                viewBox="0 0 24 24"
                aria-hidden="true"
                class="navigation__logo-icon"
            >
              <path
                  d="m16 18l6-6l-6-6M8 6l-6 6l6 6"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="2"
                  stroke-linecap="round"
                  stroke-linejoin="round"
              ></path>
            </svg>
            <span class="navigation__logo-text">ByteBattles Template</span>
          </div>
        </a>
        <button
            id="navToggle"
            type="button"
            :aria-label="isMenuOpen ? 'Закрыть меню навигации' : 'Открыть меню навигации'"
            aria-controls="navMenu"
            :aria-expanded="isMenuOpen"
            @click="toggleMenu"
            class="navigation__toggle"
        >
          <svg
              width="24"
              xmlns="http://www.w3.org/2000/svg"
              height="24"
              viewBox="0 0 24 24"
              aria-hidden="true"
              :class="['navigation-navigationtoggle-icon1', 'navigation__toggle-icon--menu', { 'navigation__toggle-icon--hidden': isMenuOpen }]"
          >
            <path
                d="M4 5h16M4 12h16M4 19h16"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
            ></path>
          </svg>
          <svg
              width="24"
              xmlns="http://www.w3.org/2000/svg"
              height="24"
              viewBox="0 0 24 24"
              aria-hidden="true"
              :class="['navigation-navigationtoggle-icon2', { 'navigation__toggle-icon--hidden': !isMenuOpen }]"
          >
            <path
                d="M20 13c0 5-3.5 7.5-7.66 8.95a1 1 0 0 1-.67-.01C7.5 20.5 4 18 4 13V6a1 1 0 0 1 1-1c2 0 4.5-1.2 6.24-2.72a1.17 1.17 0 0 1 1.52 0C14.51 3.81 17 5 19 5a1 1 0 0 1 1 1zm-5.5-3.5l-5 5m0-5l5 5"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
            ></path>
          </svg>
        </button>
        <div
            id="navMenu"
            :class="['navigation__menu', { 'navigation__menu--open': isMenuOpen }]"
            @click="handleMenuClick"
        >
          <ul class="navigation__list">
            <li class="navigation__item">
              <router-link to="/task-template-builder">
                <div class="navigation__link">
                  <svg
                      width="24"
                      xmlns="http://www.w3.org/2000/svg"
                      height="24"
                      viewBox="0 0 24 24"
                      aria-hidden="true"
                      class="navigation__link-icon"
                  >
                    <path
                        d="m16 18l6-6l-6-6M8 6l-6 6l6 6"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                    ></path>
                  </svg>
                  <span>Создать Шаблон</span>
                </div>
              </router-link>
            </li>
            <li class="navigation__item">
              <router-link to="/students">
                <div class="navigation__link">
                  <svg
                      width="24"
                      xmlns="http://www.w3.org/2000/svg"
                      height="24"
                      viewBox="0 0 24 24"
                      aria-hidden="true"
                      class="navigation__link-icon"
                  >
                    <g
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                    >
                      <path d="M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2"></path>
                      <circle r="4" cx="12" cy="7"></circle>
                    </g>
                  </svg>
                  <span>Пользователи</span>
                </div>
              </router-link>
            </li>
            <li class="navigation__item">
              <router-link to="/tasks">
                <div class="navigation__link">
                  <svg
                      width="24"
                      xmlns="http://www.w3.org/2000/svg"
                      height="24"
                      viewBox="0 0 24 24"
                      aria-hidden="true"
                      class="navigation__link-icon"
                  >
                    <g
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                    >
                      <path d="M9 5H7a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-2"></path>
                      <rect x="9" y="3" width="6" height="4" rx="2"></rect>
                      <path d="M9 14h.01"></path>
                      <path d="M9 17h.01"></path>
                      <path d="M12 16l1 1 3-3"></path>
                    </g>
                  </svg>
                  <span>Задачи</span>
                </div>
              </router-link>
            </li>
          </ul>
          <div class="navigation__actions">
            <div v-if="user" class="user-menu">
              <button @click="profile" class="navigation__action-btn btn-outline btn">
                {{ user.firstName }} {{ user.lastName }}
              </button>
              <button @click="logout" class="navigation__action-btn btn-outline btn">
                Выйти
              </button>
            </div>
            <div v-else class="auth-buttons">
              <button @click="navigateToAuth" class="navigation__action-btn btn-outline btn">
                Войти
              </button>
            </div>
          </div>
        </div>
      </div>
    </nav>
    <div class="navigation-container2">
      <div class="navigation-container3">
        <DangerousHTML
            html="<style>
  @media (prefers-reduced-motion: reduce) {
  .navigation__logo, .navigation__toggle, .navigation__link, .navigation__menu, .navigation__toggle-icon, .navigation__link::before {
    transition: none;
  }
  .navigation__logo:hover, .navigation__link:hover {
    transform: none;
  }
  .navigation__toggle:hover {
    transform: none;
  }
  }
  </style>"
        ></DangerousHTML>
      </div>
    </div>
    <div class="navigation-container4">
      <div class="navigation-container5">
        <DangerousHTML
            html="<script>
  // Обработчик изменения размера окна для автоматического закрытия меню
  (function(){
    const mediaQuery = window.matchMedia('(min-width: 992px)')

    function handleViewportChange(e) {
      if (e.matches) {
        // На больших экранах меню должно быть закрыто
        const navMenu = document.getElementById('navMenu')
        const navToggle = document.getElementById('navToggle')
        if (navMenu && navToggle) {
          navMenu.classList.remove('navigation__menu--open')
          navToggle.setAttribute('aria-expanded', 'false')
          navToggle.setAttribute('aria-label', 'Открыть меню навигации')
          document.body.style.overflow = ''
        }
      }
    }

    mediaQuery.addEventListener('change', handleViewportChange)

    // Инициализация при загрузке
    handleViewportChange(mediaQuery)
  })()
  </script>"
        ></DangerousHTML>
      </div>
    </div>
  </div>
</template>

<script>
import DangerousHTML from 'dangerous-html/vue'
import { authUtils } from '@/utils/auth'

export default {
  name: 'Navigation',
  data() {
    return {
      user: null,
      isMenuOpen: false
    }
  },
  mounted() {
    this.checkAuth()
    this.setupMediaQueryListener()

    // Слушаем изменения аутентификации
    window.addEventListener('authStateChanged', this.checkAuth)
    window.addEventListener('storage', this.checkAuth)

    // Слушаем события обновления пользователя через глобальную шину
    this.$root.$on('userUpdated', this.checkAuth)
  },
  beforeUnmount() {
    window.removeEventListener('authStateChanged', this.checkAuth)
    window.removeEventListener('storage', this.checkAuth)
    this.$root.$off('userUpdated', this.checkAuth)
  },
  methods: {
    checkAuth() {
      console.log('Checking auth in Navigation...')
      this.user = authUtils.getUser()
      console.log('Current user:', this.user)
    },

    toggleMenu() {
      this.isMenuOpen = !this.isMenuOpen
      document.body.style.overflow = this.isMenuOpen ? 'hidden' : ''
    },

    handleMenuClick(event) {
      if (event.target.closest('.navigation__link')) {
        this.isMenuOpen = false
        document.body.style.overflow = ''
      }
    },

    async profile() {
      if (!this.user) {
        this.navigateToAuth()

        return
      }

      this.closeMenu()
      await this.$router.push('/me')
    },

    navigateToAuth() {
      this.closeMenu()
      this.$router.push('/auth')
    },

    logout() {
      if (confirm('Вы уверены, что хотите выйти?')) {
        authUtils.clearAuth()
        authUtils.notifyAuthChange() // Уведомляем об изменении
        this.user = null
        this.closeMenu()

        // Показываем уведомление
        this.showNotification('Вы успешно вышли из системы')

        // Редирект если находимся на защищенной странице
        if (this.$route.meta && this.$route.meta.requiresAuth) {
          this.$router.push('/')
        }

        // Отправляем событие об обновлении пользователя
        this.$root.$emit('userUpdated')
      }
    },

    closeMenu() {
      this.isMenuOpen = false
      document.body.style.overflow = ''
    },

    showNotification(message, type = 'success') {
      const notification = document.createElement('div')

      const colors = {
        success: {
          background: 'var(--color-success, #10B981)',
          icon: '✅'
        },
        error: {
          background: 'var(--color-error, #EF4444)',
          icon: '❌'
        },
        warning: {
          background: 'var(--color-warning, #F59E0B)',
          icon: '⚠️'
        },
        info: {
          background: 'var(--color-info, #3B82F6)',
          icon: 'ℹ️'
        }
      }

      const config = colors[type] || colors.info

      notification.style.cssText = `
    position: fixed;
    top: 20px;
    right: 20px;
    background: var(--color-surface-elevated);
    color: var(--color-on-surface);
    padding: 16px 20px;
    border-radius: var(--border-radius-lg);
    z-index: 10000;
    animation: slideIn 0.3s var(--animation-curve-primary);
    max-width: 400px;
    word-wrap: break-word;
    box-shadow: var(--shadow-level-3);
    border: 1px solid var(--color-border);
    font-family: var(--font-family-body);
    font-size: var(--font-size-base);
    font-weight: var(--font-weight-body);
    line-height: var(--line-height-body);
    cursor: pointer;
    transition: all var(--animation-duration-standard) var(--animation-curve-primary);
    display: flex;
    align-items: flex-start;
    gap: 12px;
    backdrop-filter: blur(10px);
  `

      // Создаем иконку с акцентным цветом
      const iconSpan = document.createElement('span')
      iconSpan.style.cssText = `
    font-size: 18px;
    flex-shrink: 0;
    margin-top: 1px;
    color: ${config.background};
  `
      iconSpan.textContent = config.icon

      // Создаем текстовый контент
      const textSpan = document.createElement('span')
      textSpan.style.cssText = `
    flex: 1;
    color: var(--color-on-surface);
  `
      textSpan.textContent = message

      // Добавляем акцентную полоску слева
      const accentBar = document.createElement('div')
      accentBar.style.cssText = `
    position: absolute;
    left: 0;
    top: 0;
    bottom: 0;
    width: 4px;
    background: ${config.background};
    border-radius: var(--border-radius-lg) 0 0 var(--border-radius-lg);
  `

      notification.appendChild(accentBar)
      notification.appendChild(iconSpan)
      notification.appendChild(textSpan)

      // Добавляем стили для анимации в стиле вашего сайта
      const style = document.createElement('style')
      style.textContent = `
    @keyframes slideIn {
      from {
        opacity: 0;
        transform: translateX(100%) translateY(-10px) scale(0.95);
      }
      to {
        opacity: 1;
        transform: translateX(0) translateY(0) scale(1);
      }
    }

    @keyframes slideOut {
      to {
        opacity: 0;
        transform: translateX(100%) translateY(-10px) scale(0.95);
      }
    }
  `

      // Проверяем, не добавлены ли уже стили
      if (!document.getElementById('notification-styles')) {
        style.id = 'notification-styles'
        document.head.appendChild(style)
      } else {
        document.getElementById('notification-styles').appendChild(document.createTextNode(style.textContent))
      }

      document.body.appendChild(notification)

      // Закрытие по клику
      const closeNotification = () => {
        notification.style.animation = 'slideOut 0.25s var(--animation-curve-primary) forwards'
        setTimeout(() => {
          if (notification.parentNode) {
            notification.parentNode.removeChild(notification)
          }
        }, 250)
      }

      notification.addEventListener('click', closeNotification)

      // Эффект при наведении в стиле retro-card
      notification.addEventListener('mouseenter', () => {
        notification.style.transform = 'translateY(-2px)'
        notification.style.boxShadow = 'var(--shadow-level-4)'
        notification.style.borderColor = 'var(--color-primary)'
      })

      notification.addEventListener('mouseleave', () => {
        notification.style.transform = 'translateY(0)'
        notification.style.boxShadow = 'var(--shadow-level-3)'
        notification.style.borderColor = 'var(--color-border)'
      })

      // Авто-закрытие
      const autoCloseTimeout = setTimeout(closeNotification, 5000)

      // Останавливаем авто-закрытие при наведении
      notification.addEventListener('mouseenter', () => {
        clearTimeout(autoCloseTimeout)
      })

      notification.addEventListener('mouseleave', () => {
        setTimeout(closeNotification, 5000)
      })
    },

    setupMediaQueryListener() {
      const mediaQuery = window.matchMedia('(min-width: 992px)')
      const handleViewportChange = (e) => {
        if (e.matches && this.isMenuOpen) {
          this.closeMenu()
        }
      }
      mediaQuery.addEventListener('change', handleViewportChange)
    }
  }
}
</script>

<style scoped>
/* Ваши существующие стили остаются без изменений */
.navigation-container1 {
  display: contents;
}

.navigation-navigationtoggle-icon1 {
  color: var(--color-on-surface);
  width: 24px;
  height: 24px;
  position: absolute;
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
}

.navigation-navigationtoggle-icon2 {
  color: var(--color-on-surface);
  width: 24px;
  height: 24px;
  position: absolute;
  transform: rotate(180deg) scale(0.8);
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
}

.navigation__toggle-icon--hidden {
  opacity: 0;
  transform: rotate(180deg) scale(0.8);
}

.navigation-navigationtoggle-icon1:not(.navigation__toggle-icon--hidden) {
  opacity: 1;
  transform: scale(1);
}

.navigation-navigationtoggle-icon2:not(.navigation__toggle-icon--hidden) {
  opacity: 1;
  transform: rotate(0deg) scale(1);
}

.navigation-container2 {
  display: none;
}

.navigation-container3 {
  display: contents;
}

.navigation-container4 {
  display: none;
}

.navigation-container5 {
  display: contents;
}

/* Стили для мобильного меню */
@media (max-width: 991px) {
  .navigation__menu {
    display: none;
    position: absolute;
    top: 100%;
    left: 0;
    right: 0;
    background: var(--color-surface);
    border-top: 1px solid var(--color-border);
    box-shadow: var(--shadow-lg);
    padding: var(--spacing-lg);
    flex-direction: column;
    gap: var(--spacing-md);
  }

  .navigation__menu--open {
    display: flex;
  }

  .navigation__list {
    flex-direction: column;
    gap: var(--spacing-sm);
  }

  .navigation__actions {
    margin-top: var(--spacing-md);
    border-top: 1px solid var(--color-border);
    padding-top: var(--spacing-md);
  }

  .user-menu,
  .auth-buttons {
    display: flex;
    flex-direction: column;
    gap: var(--spacing-md);
    width: 100%;
  }

  .navigation__action-btn {
    width: 100%;
    justify-content: center;
  }

  .user-menu .navigation__action-btn:not(:last-child) {
    margin-bottom: var(--spacing-sm);
  }
}

@media (min-width: 992px) {
  .navigation__toggle {
    display: none;
  }

  .navigation__menu {
    display: flex !important;
    position: static;
    flex-direction: row;
    align-items: center;
    gap: var(--spacing-xl);
    background: transparent;
    border: none;
    box-shadow: none;
    padding: 0;
  }

  .navigation__list {
    flex-direction: row;
    gap: var(--spacing-lg);
  }

  .navigation__actions {
    margin-top: 0;
    border-top: none;
    padding-top: 0;
  }

  .user-menu,
  .auth-buttons {
    display: flex;
    gap: var(--spacing-md);
    width: 100%;
  }

  .user-menu .navigation__action-btn:first-child {
    background: transparent;
    border-color: transparent;
    cursor: default;
  }

  .user-menu .navigation__action-btn:first-child:hover {
    background: transparent;
    border-color: transparent;
  }
}
</style>