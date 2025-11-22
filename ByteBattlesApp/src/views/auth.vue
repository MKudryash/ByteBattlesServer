<template>
  <div class="auth-container1">
    <div class="auth-container2">
      <div class="auth-container3">
        <DangerousHTML
            html="<style>
  @media (prefers-reduced-motion: reduce) {
  .auth-card, .auth-input, .auth-btn, .auth-toggle, .auth-social-btn {
    transition: none;
  }
  .auth-decoration-left, .auth-decoration-right {
    animation: none;
  }
  }
  </style>"
        ></DangerousHTML>
      </div>
    </div>
    <main id="auth-main" class="auth">
      <div aria-hidden="true" class="auth-background-texture"></div>
      <div class="auth-container">
        <div class="auth-header">
          <div class="auth-brand">
            <svg
                width="32"
                xmlns="http://www.w3.org/2000/svg"
                height="32"
                viewBox="0 0 24 24"
                class="auth-logo-icon"
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
            <span class="auth-logo-text">CodeCraft Template</span>
          </div>
          <p class="auth-tagline">
            Войдите в свою мастерскую создания учебных задач
            <span v-html="raw1tjg"></span>
          </p>
        </div>

        <div class="auth-card">
          <div class="auth-tabs">
            <button
                @click="setActiveTab('login')"
                :class="['auth-tab', { 'auth-tab--active': activeTab === 'login' }]"
                aria-controls="login-form"
                aria-selected="true"
            >
              <svg width="20" xmlns="http://www.w3.org/2000/svg" height="20" viewBox="0 0 24 24">
                <g fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4"></path>
                  <path d="m10 17l5-5l-5-5"></path>
                  <path d="M15 12H3"></path>
                </g>
              </svg>
              <span>Вход</span>
            </button>
            <button
                @click="setActiveTab('register')"
                :class="['auth-tab', { 'auth-tab--active': activeTab === 'register' }]"
                aria-controls="register-form"
                aria-selected="false"
            >
              <svg width="20" xmlns="http://www.w3.org/2000/svg" height="20" viewBox="0 0 24 24">
                <g fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"></path>
                  <circle r="4" cx="9" cy="7"></circle>
                  <path d="M19 8v6m3-3h-6"></path>
                </g>
              </svg>
              <span>Регистрация</span>
            </button>
          </div>

          <!-- Форма входа -->
          <form
              v-if="activeTab === 'login'"
              id="login-form"
              @submit.prevent="handleLogin"
              class="auth-form"
          >
            <div class="form-group">
              <label for="login-email" class="auth-label">
                <svg width="18" xmlns="http://www.w3.org/2000/svg" height="18" viewBox="0 0 24 24">
                  <g fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                    <path d="m22 7l-8.991 5.727a2 2 0 0 1-2.009 0L2 7"></path>
                    <rect x="2" y="4" rx="2" width="20" height="16"></rect>
                  </g>
                </svg>
                <span>Email</span>
              </label>
              <input
                  v-model="loginForm.email"
                  type="email"
                  id="login-email"
                  required
                  placeholder="your.email@example.com"
                  class="auth-input"
              >
            </div>

            <div class="form-group">
              <label for="login-password" class="auth-label">
                <svg width="18" xmlns="http://www.w3.org/2000/svg" height="18" viewBox="0 0 24 24">
                  <path
                      d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10"
                      fill="none"
                      stroke="currentColor"
                      stroke-width="2"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                  ></path>
                </svg>
                <span>Пароль</span>
              </label>
              <input
                  v-model="loginForm.password"
                  type="password"
                  id="login-password"
                  required
                  placeholder="••••••••"
                  class="auth-input"
              >
            </div>

            <div class="auth-options">
              <label class="auth-checkbox">
                <input v-model="loginForm.rememberMe" type="checkbox" class="auth-checkbox-input">
                <span class="auth-checkbox-label">Запомнить меня</span>
              </label>
              <a href="#forgot-password" class="auth-link">
                <span>Забыли пароль?</span>
              </a>
            </div>

            <button
                type="submit"
                :disabled="loading"
                class="auth-btn btn-primary btn"
            >
              <span v-if="loading" class="auth-btn-loading">
                <svg width="18" xmlns="http://www.w3.org/2000/svg" height="18" viewBox="0 0 24 24" class="auth-spinner">
                  <path
                      d="M21 12a9 9 0 1 1-6.219-8.56"
                      fill="none"
                      stroke="currentColor"
                      stroke-width="2"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                  ></path>
                </svg>
                Вход...
              </span>
              <span v-else>
                <svg width="18" xmlns="http://www.w3.org/2000/svg" height="18" viewBox="0 0 24 24">
                  <g fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                    <path d="M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4"></path>
                    <path d="m10 17l5-5l-5-5"></path>
                    <path d="M15 12H3"></path>
                  </g>
                </svg>
                Войти в систему
              </span>
            </button>
          </form>

          <!-- Форма регистрации -->
          <form
              v-if="activeTab === 'register'"
              id="register-form"
              @submit.prevent="handleRegister"
              class="auth-form"
          >
            <div class="form-row">
              <div class="form-group">
                <label for="register-firstName" class="auth-label">
                  <svg width="18" xmlns="http://www.w3.org/2000/svg" height="18" viewBox="0 0 24 24">
                    <g fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"
                       stroke-linejoin="round">
                      <path d="M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2"></path>
                      <circle r="4" cx="12" cy="7"></circle>
                    </g>
                  </svg>
                  <span>Имя</span>
                </label>
                <input
                    v-model="registerForm.firstName"
                    type="text"
                    id="register-firstName"
                    required
                    placeholder="Иван"
                    class="auth-input"
                >
              </div>
              <div class="form-group">
                <label for="register-lastName" class="auth-label">
                  <span>Фамилия</span>
                </label>
                <input
                    v-model="registerForm.lastName"
                    type="text"
                    id="register-lastName"
                    required
                    placeholder="Петров"
                    class="auth-input"
                >
              </div>
            </div>

            <div class="form-group">
              <label for="register-email" class="auth-label">
                <svg width="18" xmlns="http://www.w3.org/2000/svg" height="18" viewBox="0 0 24 24">
                  <g fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                    <path d="m22 7l-8.991 5.727a2 2 0 0 1-2.009 0L2 7"></path>
                    <rect x="2" y="4" rx="2" width="20" height="16"></rect>
                  </g>
                </svg>
                <span>Email</span>
              </label>
              <input
                  v-model="registerForm.email"
                  type="email"
                  id="register-email"
                  required
                  placeholder="your.email@example.com"
                  class="auth-input"
              >
            </div>

            <div class="form-group">
              <label for="register-password" class="auth-label">
                <svg width="18" xmlns="http://www.w3.org/2000/svg" height="18" viewBox="0 0 24 24">
                  <path
                      d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10"
                      fill="none"
                      stroke="currentColor"
                      stroke-width="2"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                  ></path>
                </svg>
                <span>Пароль</span>
              </label>
              <input
                  v-model="registerForm.password"
                  type="password"
                  id="register-password"
                  required
                  placeholder="••••••••"
                  class="auth-input"
              >
            </div>

            <div class="form-group">
              <label for="register-confirmPassword" class="auth-label">
                <svg width="18" xmlns="http://www.w3.org/2000/svg" height="18" viewBox="0 0 24 24">
                  <path
                      d="M20 6L9 17l-5-5"
                      fill="none"
                      stroke="currentColor"
                      stroke-width="2"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                  ></path>
                </svg>
                <span>Подтверждение пароля</span>
              </label>
              <input
                  v-model="registerForm.confirmPassword"
                  type="password"
                  id="register-confirmPassword"
                  required
                  placeholder="••••••••"
                  class="auth-input"
              >
            </div>

            <label class="auth-checkbox">
              <input v-model="registerForm.agreeTerms" type="checkbox" class="auth-checkbox-input" required>
              <span class="auth-checkbox-label">
                Я согласен с 
                <a href="#terms" class="auth-link">условиями использования</a>
                и 
                <a href="#privacy" class="auth-link">политикой конфиденциальности</a>
              </span>
            </label>

            <button
                type="submit"
                :disabled="loading"
                class="auth-btn btn-accent btn"
            >
              <span v-if="loading" class="auth-btn-loading">
                <svg width="18" xmlns="http://www.w3.org/2000/svg" height="18" viewBox="0 0 24 24" class="auth-spinner">
                  <path
                      d="M21 12a9 9 0 1 1-6.219-8.56"
                      fill="none"
                      stroke="currentColor"
                      stroke-width="2"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                  ></path>
                </svg>
                Регистрация...
              </span>
              <span v-else>
                <svg width="18" xmlns="http://www.w3.org/2000/svg" height="18" viewBox="0 0 24 24">
                  <g fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                    <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"></path>
                    <circle r="4" cx="9" cy="7"></circle>
                    <path d="M19 8v6m3-3h-6"></path>
                  </g>
                </svg>
                Создать аккаунт
              </span>
            </button>
          </form>

          <div class="auth-divider">
            <span class="auth-divider-text">или продолжите с помощью</span>
          </div>

          <div class="auth-social">
            <button @click="socialLogin('google')" class="auth-social-btn btn-outline btn">
              <svg width="18" xmlns="http://www.w3.org/2000/svg" height="18" viewBox="0 0 24 24">
                <path
                    d="M21 12a9 9 0 1 1-9-9c2.5 0 4.5 1 6 2l-3 3"
                    fill="none"
                    stroke="currentColor"
                    stroke-width="2"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                ></path>
              </svg>
              <span>Google</span>
            </button>
            <button @click="socialLogin('github')" class="auth-social-btn btn-outline btn">
              <svg width="18" xmlns="http://www.w3.org/2000/svg" height="18" viewBox="0 0 24 24">
                <g fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path
                      d="M9 19c-5 1.5-5-2.5-7-3m14 6v-3.87a3.37 3.37 0 0 0-.94-2.61c3.14-.35 6.44-1.54 6.44-7A5.44 5.44 0 0 0 20 4.77A5.07 5.07 0 0 0 19.91 1S18.73.65 16 2.48a13.38 13.38 0 0 0-7 0C6.27.65 5.09 1 5.09 1A5.07 5.07 0 0 0 5 4.77a5.44 5.44 0 0 0-1.5 3.78c0 5.42 3.3 6.61 6.44 7A3.37 3.37 0 0 0 9 18.13V22"
                  ></path>
                </g>
              </svg>
              <span>GitHub</span>
            </button>
          </div>
        </div>

        <div class="auth-footer">
          <p class="auth-copyright">
            <svg width="16" xmlns="http://www.w3.org/2000/svg" height="16" viewBox="0 0 24 24">
              <path
                  d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="2"
                  stroke-linecap="round"
                  stroke-linejoin="round"
              ></path>
            </svg>
            <span>© 2025 CodeCraft Template. Ваши данные в безопасности.</span>
          </p>
        </div>
      </div>
      <div
          aria-hidden="true"
          class="auth-decoration-left auth-decoration"
      ></div>
      <div
          aria-hidden="true"
          class="auth-decoration-right auth-decoration"
      ></div>
    </main>
    <div class="auth-container4">
      <div class="auth-container5">
        <DangerousHTML
            html="<style>
          @keyframes auth-fade-in {from {opacity: 0; transform: translateY(10px);}
  to {opacity: 1; transform: translateY(0);}}@keyframes auth-slide-in {from {transform: translateX(-20px); opacity: 0;}
  to {transform: translateX(0); opacity: 1;}}
          </style> "
        ></DangerousHTML>
      </div>
    </div>
    <div class="auth-container6">
      <div class="auth-container7">
        <DangerousHTML
            html="<script defer data-name='auth'>
  (function(){
    // Smooth form transitions
    const authTabs = document.querySelectorAll('.auth-tab')
    const authForms = document.querySelectorAll('.auth-form')
  
    authTabs.forEach(tab => {
      tab.addEventListener('click', function() {
        const target = this.getAttribute('aria-controls')
        
        // Smooth transition between forms
        authForms.forEach(form => {
          if (form.id === target) {
            form.style.opacity = '0'
            form.style.transform = 'translateX(20px)'
            
            setTimeout(() => {
              form.style.display = 'block'
              setTimeout(() => {
                form.style.opacity = '1'
                form.style.transform = 'translateX(0)'
              }, 50)
            }, 150)
          } else {
            form.style.opacity = '0'
            form.style.transform = 'translateX(-20px)'
            setTimeout(() => {
              form.style.display = 'none'
            }, 150)
          }
        })
      })
    })
  
    // Input focus effects
    const authInputs = document.querySelectorAll('.auth-input')
    
    authInputs.forEach(input => {
      input.addEventListener('focus', function() {
        this.parentElement.classList.add('auth-input--focused')
      })
      
      input.addEventListener('blur', function() {
        if (!this.value) {
          this.parentElement.classList.remove('auth-input--focused')
        }
      })
    })
  
    // Social button hover effects
    const socialButtons = document.querySelectorAll('.auth-social-btn')
    
    socialButtons.forEach(btn => {
      btn.addEventListener('mouseenter', function() {
        this.style.transform = 'translateY(-2px)'
      })
      
      btn.addEventListener('mouseleave', function() {
        this.style.transform = 'translateY(0)'
      })
    })
  
    // Password strength indicator (for register form)
    const passwordInput = document.getElementById('register-password')
    if (passwordInput) {
      passwordInput.addEventListener('input', function() {
        const strength = calculatePasswordStrength(this.value)
        updatePasswordStrength(strength)
      })
    }
  
    function calculatePasswordStrength(password) {
      let strength = 0
      if (password.length >= 8) strength++
      if (/[A-Z]/.test(password)) strength++
      if (/[0-9]/.test(password)) strength++
      if (/[^A-Za-z0-9]/.test(password)) strength++
      return strength
    }
  
    function updatePasswordStrength(strength) {
      // Implementation for password strength indicator
    }
  })()
  </script>"
        ></DangerousHTML>
      </div>
    </div>
  </div>
</template>

<script>
import DangerousHTML from 'dangerous-html/vue';
import {authAPI} from '@/api/auth'
import { authUtils } from '@/utils/auth'

export default {
  name: 'Auth',
  components: {
    DangerousHTML,
  },
  data() {
    return {
      raw1tjg: ' ',
      activeTab: 'login',
      loading: false,
      loginForm: {
        email: '',
        password: '',
        rememberMe: false
      },
      registerForm: {
        firstName: '',
        lastName: '',
        email: '',
        password: '',
        confirmPassword: '',
        agreeTerms: false
      }
    }
  },
  methods: {
    setActiveTab(tab) {
      this.activeTab = tab
    },

    async handleLogin() {
      this.loading = true;
      try {
        const userData = {
          email: this.loginForm.email,
          password: this.loginForm.password,
        };

        console.log('Sending login request:', userData);

        const response = await authAPI.login(userData);

        console.log('Login response:', response);

        if (!response.ok) {
          const errorText = await response.text();
          console.error('Login failed with status:', response.status, 'Response:', errorText);

          let errorMessage = `Ошибка входа (${response.status})`;

          try {
            const errorData = JSON.parse(errorText);
            errorMessage = errorData.message || errorData.title || errorMessage;
          } catch (e) {
            errorMessage = errorText || errorMessage;
          }

          throw new Error(errorMessage);
        }

        let data;
        try {
          data = await response.json();
          console.log("Login success data:", data);
        } catch (parseError) {
          console.error('Error parsing login response:', parseError);
          throw new Error('Неверный формат ответа от сервера');
        }

        if (data.accessToken) {
          // Сохраняем данные через утилиты
          authUtils.setUser(data.user || data);
          localStorage.setItem('accessToken', data.accessToken);
          localStorage.setItem('refreshToken', data.refreshToken || '');

          if (this.loginForm.rememberMe) {
            localStorage.setItem('rememberMe', 'true');
          }

          // Уведомляем об изменении состояния аутентификации
          authUtils.notifyAuthChange();

          // Отправляем событие через глобальную шину
          this.$root.$emit('userUpdated');

          console.log('Login successful, redirecting...');

          // Проверяем, есть ли redirect параметр
          const redirect = this.$route.query.redirect || '/';
          this.$router.push(redirect);
        } else {
          throw new Error('Отсутствует токен доступа в ответе');
        }

      } catch (error) {
        console.error('Login error:', error);
        let userMessage = 'Ошибка входа';
        if (error.message.includes('Network Error') || error.message.includes('Failed to fetch')) {
          userMessage = 'Ошибка сети. Проверьте подключение к интернету.';
        } else if (error.message.includes('JSON')) {
          userMessage = 'Ошибка обработки ответа сервера.';
        } else {
          userMessage = error.message;
        }

        alert(userMessage);
      } finally {
        this.loading = false;
      }
    },

    async handleRegister() {
      if (this.registerForm.password !== this.registerForm.confirmPassword) {
        alert('Пароли не совпадают')
        return
      }

      this.loading = true

      try {
        const userData = {
          firstName: this.registerForm.firstName,
          lastName: this.registerForm.lastName,
          email: this.registerForm.email,
          password: this.registerForm.password,
          role: ["admin"]
        }

        const response = await authAPI.register(userData)

        const data = await response.json()

        if (!response.ok) {
          await this.handleErrorResponse(response.status, data)
          return
        }

        console.log("Success: ", data)

        // Сохраняем данные через утилиты
        authUtils.setUser(data.user || data);
        localStorage.setItem('accessToken', data.accessToken);
        localStorage.setItem('refreshToken', data.refreshToken);

        // Уведомляем об изменении состояния аутентификации
        authUtils.notifyAuthChange();

        // Отправляем событие через глобальную шину
        this.$root.$emit('userUpdated');

        // Показываем сообщение об успехе
        alert('Регистрация прошла успешно!')

        // Перенаправляем на главную страницу
        this.$router.push('/')

      } catch (error) {
        console.error('Registration error:', error)
        this.handleNetworkError(error)
      } finally {
        this.loading = false
      }
    },

// Метод для обработки ошибок HTTP
    async handleErrorResponse(statusCode, errorData) {
      const errorMessages = {
        400: 'Неверный запрос. Проверьте введенные данные.',
        409: 'Пользователь с таким email уже существует.',
        422: 'Ошибка валидации данных.',
        500: 'Внутренняя ошибка сервера. Попробуйте позже.',
        503: 'Сервис временно недоступен.'
      }

      // Получаем сообщение об ошибке из ответа сервера или используем стандартное
      const serverMessage = errorData?.message || errorData?.title
      const defaultMessage = errorMessages[statusCode] || `Ошибка регистрации (код: ${statusCode})`

      const finalMessage = serverMessage ? `${defaultMessage}\nДетали: ${serverMessage}` : defaultMessage

      alert(finalMessage)

      // Дополнительные действия для определенных ошибок
      switch (statusCode) {
        case 409:
          // Очищаем поле email при конфликте
          this.registerForm.email = ''
          break
        case 422:
          // Можно добавить подсветку полей с ошибками
          console.warn('Validation errors:', errorData.errors)
          break
      }
    },

// Метод для обработки сетевых ошибок
    handleNetworkError(error) {
      if (error.name === 'TypeError' && error.message.includes('fetch')) {
        alert('Ошибка сети. Проверьте подключение к интернету.')
      } else if (error.name === 'SyntaxError') {
        alert('Ошибка обработки ответа сервера.')
      } else {
        alert('Произошла непредвиденная ошибка. Попробуйте еще раз.')
      }
    },

    socialLogin(provider) {
      // Имитация социального входа
      console.log(`Social login with ${provider}`)
      // В реальном приложении здесь будет редирект на OAuth провайдера
    }
  }
}
</script>

<style scoped>
.auth-container1 {
  display: contents;
}

.auth-container2 {
  display: none;
}

.auth-container3 {
  display: contents;
}

.auth-container4 {
  display: none;
}

.auth-container5 {
  display: contents;
}

.auth-container6 {
  display: none;
}

.auth-container7 {
  display: contents;
}

/* Стили для авторизации */
.auth {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: var(--spacing-2xl);
  background: var(--color-surface);
  position: relative;
}

.auth-background-texture {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-image: repeating-linear-gradient(
      45deg,
      transparent,
      transparent 2px,
      color-mix(in srgb, var(--color-border) 5%, transparent) 2px,
      color-mix(in srgb, var(--color-border) 5%, transparent) 4px
  );
  pointer-events: none;
  z-index: 1;
  opacity: 0.2;
}

.auth-container {
  max-width: 440px;
  width: 100%;
  z-index: 2;
}

.auth-header {
  text-align: center;
  margin-bottom: var(--spacing-2xl);
}

.auth-brand {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--spacing-sm);
  margin-bottom: var(--spacing-md);
}

.auth-logo-icon {
  color: var(--color-primary);
}

.auth-logo-text {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--color-on-surface);
}

.auth-tagline {
  color: var(--color-on-surface-variant);
  font-size: 0.95rem;
  line-height: 1.5;
}

.auth-card {
  background: var(--color-surface-container);
  border: 1px solid var(--color-border);
  border-radius: var(--border-radius-lg);
  padding: var(--spacing-2xl);
  box-shadow: var(--shadow-md);
}

.auth-tabs {
  display: flex;
  background: var(--color-surface-variant);
  border-radius: var(--border-radius-md);
  padding: var(--spacing-xs);
  margin-bottom: var(--spacing-xl);
}

.auth-tab {
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
  color: var(--color-on-surface-variant);
}

.auth-tab--active {
  background: var(--color-surface);
  color: var(--color-primary);
  box-shadow: var(--shadow-sm);
}

.auth-form {
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-md);
}

.form-group {
  margin-bottom: var(--spacing-lg);
}

.auth-label {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  margin-bottom: var(--spacing-sm);
  font-weight: 500;
  color: var(--color-on-surface);
  font-size: 0.9rem;
}

.auth-input {
  width: 100%;
  padding: var(--spacing-md) var(--spacing-lg);
  border: 1px solid var(--color-border);
  border-radius: var(--border-radius-md);
  background: var(--color-surface);
  color: var(--color-on-surface);
  transition: all var(--animation-duration-standard) var(--animation-curve-primary);
}

.auth-input:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 2px color-mix(in srgb, var(--color-primary) 20%, transparent);
}

.auth-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-xl);
}

.auth-checkbox {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  cursor: pointer;
}

.auth-checkbox-input {
  width: 16px;
  height: 16px;
}

.auth-checkbox-label {
  font-size: 0.9rem;
  color: var(--color-on-surface-variant);
}

.auth-link {
  color: var(--color-primary);
  text-decoration: none;
  font-size: 0.9rem;
  transition: color var(--animation-duration-standard) var(--animation-curve-primary);
}

.auth-link:hover {
  color: var(--color-primary-dark);
  text-decoration: underline;
}

.auth-btn {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--spacing-sm);
  padding: var(--spacing-lg);
  margin-bottom: var(--spacing-xl);
}

.auth-btn-loading {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.auth-spinner {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

.auth-divider {
  position: relative;
  text-align: center;
  margin-bottom: var(--spacing-xl);
}

.auth-divider::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 0;
  right: 0;
  height: 1px;
  background: var(--color-border);
}

.auth-divider-text {
  background: var(--color-surface-container);
  padding: 0 var(--spacing-md);
  color: var(--color-on-surface-variant);
  font-size: 0.9rem;
  position: relative;
  z-index: 1;
}

.auth-social {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-md);
}

.auth-social-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--spacing-sm);
  padding: var(--spacing-md);
}

.auth-footer {
  text-align: center;
  margin-top: var(--spacing-2xl);
}

.auth-copyright {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--spacing-sm);
  color: var(--color-on-surface-variant);
  font-size: 0.85rem;
}

.auth-decoration {
  position: absolute;
  width: 200px;
  height: 200px;
  opacity: 0.1;
  pointer-events: none;
}

.auth-decoration-left {
  left: 10%;
  bottom: 20%;
  background: var(--color-primary);
  border-radius: 50%;
  filter: blur(40px);
}

.auth-decoration-right {
  right: 10%;
  top: 20%;
  background: var(--color-accent);
  border-radius: 50%;
  filter: blur(40px);
}

@media (max-width: 768px) {
  .auth {
    padding: var(--spacing-lg);
  }

  .auth-card {
    padding: var(--spacing-xl);
  }

  .form-row {
    grid-template-columns: 1fr;
  }

  .auth-social {
    grid-template-columns: 1fr;
  }
}
</style>