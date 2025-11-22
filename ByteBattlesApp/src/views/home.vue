<template>
  <div class="home-container">
    <app-navigation></app-navigation>

    <!-- Hero секция -->
    <section class="hero-section" aria-label="Главная секция">
      <div class="hero-background">
        <div class="hero-particles"></div>
      </div>
      <div class="container">
        <div class="hero-content">
          <div class="hero-text">
            <div class="hero-badge retro-card">
              <span class="badge-icon">🚀</span>
              Образовательная платформа нового поколения
            </div>
            <h1 class="hero-title">
              Создавайте и решайте
              <span class="title-gradient">учебные задачи</span>
              по программированию
            </h1>
            <p class="hero-description">
              Интерактивная платформа для преподавателей.
              Создавайте структурированные задания, автоматически проверяйте решения
              и отслеживайте прогресс студентов в реальном времени.
            </p>
            <div class="hero-actions">
              <router-link to="/task-template-builder" class="btn-primary btn-large">
                <span class="btn-icon">⚡</span>
                Создать задачу
              </router-link>
            </div>
          </div>
          <div class="hero-visual">
            <div class="code-preview retro-card">
              <div class="code-header">
                <div class="code-dots">
                  <span></span>
                  <span></span>
                  <span></span>
                </div>
                <span class="code-language">python</span>
              </div>
              <div class="code-content">
                <pre><code>def find_max(numbers):
    """Найдите максимальное число в массиве"""
    return max(numbers) if numbers else None

# Автоматическая проверка
test_cases = [
    ([1, 5, 3, 9, 2], 9),
    ([], None),
    ([-1, -5, -2], -1)
]</code></pre>
              </div>
            </div>
            <div class="floating-elements">
              <div class="floating-card card-1 retro-card">
                <span class="card-icon">✅</span>
                <span>Автопроверка</span>
              </div>
              <div class="floating-card card-2 retro-card">
                <span class="card-icon">⚡</span>
                <span>Мгновенная обратная связь</span>
              </div>
              <div class="floating-card card-3 retro-card">
                <span class="card-icon">📊</span>
                <span>Детальная статистика</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>


    <!-- Поддерживаемые языки -->
    <section class="languages-section" aria-label="Поддерживаемые языки программирования">
      <div class="container">
        <div class="section-header">
          <h2 class="section-title">Поддерживаемые языки программирования</h2>
          <p class="section-subtitle">Широкая поддержка популярных и современных языков</p>
        </div>
        <div class="languages-grid">
          <div class="language-card retro-card" v-for="lang in popularLanguages" :key="lang.name">
            <div class="lang-icon">{{ lang.icon }}</div>
            <div class="lang-info">
              <h3>{{ lang.name }}</h3>
              <p>{{ lang.description }}</p>
              <div class="lang-features">
                <span class="lang-feature" v-for="feature in lang.features" :key="feature">
                  {{ feature }}
                </span>
              </div>
            </div>
          </div>
        </div>
        <div class="languages-cta">
          <p>Не нашли нужный язык? <a href="#" class="link">Предложить добавление</a></p>
        </div>
      </div>
    </section>

    <app-footer></app-footer>
  </div>
</template>

<script>
import AppFooter from "@/components/footer.vue";
import AppNavigation from "@/components/navigation.vue";

export default {
  name: 'Home',
  components: {AppNavigation, AppFooter},
  data() {
    return {
      popularLanguages: [
        {
          name: 'Python',
          icon: '🐍',
          description: 'Идеален для начинающих и data science',
          features: ['NumPy', 'Pandas', 'Matplotlib']
        },
        {
          name: 'JavaScript',
          icon: '📜',
          description: 'Веб-разработка и современные приложения',
          features: ['Node.js', 'React', 'Vue.js']
        },
        {
          name: 'Java',
          icon: '☕',
          description: 'Корпоративные приложения и Android',
          features: ['Spring', 'JUnit', 'Maven']
        },
        {
          name: 'C++',
          icon: '⚡',
          description: 'Высокопроизводительные вычисления',
          features: ['STL', 'Boost', 'Qt']
        },
        {
          name: 'TypeScript',
          icon: '🔷',
          description: 'Типизированный JavaScript',
          features: ['Type Checking', 'Modern ES']
        },
        {
          name: 'Go',
          icon: '🐹',
          description: 'Простота и высокая производительность',
          features: ['Goroutines', 'Channels']
        }
      ]
    }
  },
  metaInfo: {
    title: 'Sarcastic Affectionate Gerbil - Образовательная платформа',
    meta: [
      {
        property: 'og:title',
        content: 'Sarcastic Affectionate Gerbil - Образовательная платформа',
      },
      {
        name: 'description',
        content: 'Интерактивная платформа для создания и решения учебных задач по программированию. Автоматическая проверка, аналитика прогресса, поддержка 15+ языков.'
      }
    ],
  },
}
</script>

<style scoped>
.home-container {
  width: 100%;
  min-height: 100vh;
  background: var(--color-surface);
}

/* Hero секция */
.hero-section {
  position: relative;
  padding: var(--spacing-4xl) var(--spacing-xl);
  background: linear-gradient(135deg,
  color-mix(in srgb, var(--color-primary) 5%, transparent) 0%,
  color-mix(in srgb, var(--color-secondary) 8%, transparent) 100%);
  overflow: hidden;
}

.hero-background {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  pointer-events: none;
}

.hero-particles {
  position: absolute;
  width: 100%;
  height: 100%;
  background-image:
      radial-gradient(circle at 20% 80%, color-mix(in srgb, var(--color-primary) 8%, transparent) 0%, transparent 50%),
      radial-gradient(circle at 80% 20%, color-mix(in srgb, var(--color-secondary) 6%, transparent) 0%, transparent 50%);
}

.hero-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-2xl);
  align-items: center;
  position: relative;
  z-index: 2;
}

.hero-badge {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: var(--spacing-sm) var(--spacing-lg);
  margin-bottom: var(--spacing-xl);
  background: var(--color-surface-elevated);
  border: 1px solid var(--color-border);
}

.badge-icon {
  font-size: var(--font-size-lg);
}

.hero-title {
  font-size: var(--font-size-hero);
  font-weight: var(--font-weight-heading);
  line-height: var(--line-height-heading);
  color: var(--color-on-surface);
  margin-bottom: var(--spacing-lg);
  font-family: var(--font-family-heading);
}

.title-gradient {
  background: linear-gradient(135deg, var(--color-primary), var(--color-secondary));
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.hero-description {
  font-size: var(--font-size-lg);
  line-height: var(--line-height-body);
  color: var(--color-on-surface-secondary);
  margin-bottom: var(--spacing-2xl);
  max-width: 90%;
}

.hero-actions {
  display: flex;
  gap: var(--spacing-lg);
  margin-bottom: var(--spacing-2xl);
}

.btn-large {
  padding: var(--spacing-lg) var(--spacing-2xl);
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-heading);
}

.hero-stats {
  display: flex;
  gap: var(--spacing-2xl);
}

.stat {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}

.stat-number {
  font-size: var(--font-size-xl);
  font-weight: var(--font-weight-heading);
  color: var(--color-primary);
  font-family: var(--font-family-heading);
}

.stat-label {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
}

/* Hero визуальная часть */
.hero-visual {
  position: relative;
}

.code-preview {
  background: var(--color-surface-elevated);
  border: 1px solid var(--color-border);
  border-radius: var(--border-radius-lg);
  overflow: hidden;
  box-shadow: var(--shadow-level-3);
}

.code-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-md);
  background: var(--color-backplate);
  border-bottom: 1px solid var(--color-border);
}

.code-dots {
  display: flex;
  gap: var(--spacing-xs);
}

.code-dots span {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background: var(--color-border);
}

.code-dots span:nth-child(1) { background: var(--color-accent); }
.code-dots span:nth-child(2) { background: var(--color-primary); }
.code-dots span:nth-child(3) { background: var(--color-secondary); }

.code-language {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
  font-family: monospace;
}

.code-content {
  padding: var(--spacing-lg);
}

.code-content pre {
  margin: 0;
  font-family: 'Monaco', 'Consolas', monospace;
  font-size: var(--font-size-sm);
  line-height: var(--line-height-body);
  color: var(--color-on-surface);
}

.floating-elements {
  position: absolute;
  top: 50%;
  left: -20px;
  right: -20px;
  bottom: -20px;
  pointer-events: none;
}

.floating-card {
  position: absolute;
  padding: var(--spacing-md);
  background: var(--color-surface-elevated);
  border: 1px solid var(--color-border);
  border-radius: var(--border-radius-md);
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  font-size: var(--font-size-sm);
  white-space: nowrap;
  box-shadow: var(--shadow-level-2);
  animation: float 6s ease-in-out infinite;
}

.card-1 {
  top: 10%;
  left: 10%;
  animation-delay: 0s;
}

.card-2 {
  top: 60%;
  right: 5%;
  animation-delay: 2s;
}

.card-3 {
  bottom: 20%;
  left: 5%;
  animation-delay: 4s;
}

@keyframes float {
  0%, 100% { transform: translateY(0px) rotate(0deg); }
  50% { transform: translateY(-10px) rotate(1deg); }
}

/* Секции */
.features-section,
.workflow-section,
.languages-section,
.cta-section {
  padding: var(--spacing-4xl) 0;
}

.section-header {
  text-align: center;
  margin-bottom: var(--spacing-3xl);
}

.section-title {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
  margin-bottom: var(--spacing-lg);
  font-family: var(--font-family-heading);
}

.section-subtitle {
  font-size: var(--font-size-lg);
  color: var(--color-on-surface-secondary);
  max-width: 600px;
  margin: 0 auto;
  line-height: var(--line-height-body);
}

/* Особенности */
.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: var(--spacing-xl);
}

.feature-card {
  padding: var(--spacing-2xl);
  transition: transform var(--animation-duration-standard) var(--animation-curve-primary);
}

.feature-card:hover {
  transform: translateY(-5px);
}

.feature-icon {
  font-size: var(--font-size-2xl);
  margin-bottom: var(--spacing-lg);
}

.feature-card h3 {
  font-size: var(--font-size-lg);
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
  margin-bottom: var(--spacing-md);
  font-family: var(--font-family-heading);
}

.feature-card p {
  color: var(--color-on-surface-secondary);
  margin-bottom: var(--spacing-lg);
  line-height: var(--line-height-body);
}

.feature-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.feature-list li {
  padding: var(--spacing-xs) 0;
  color: var(--color-on-surface);
  position: relative;
  padding-left: var(--spacing-lg);
}

.feature-list li::before {
  content: '✓';
  position: absolute;
  left: 0;
  color: var(--color-primary);
  font-weight: var(--font-weight-heading);
}

/* Workflow */
.workflow-steps {
  display: grid;
  gap: var(--spacing-xl);
  max-width: 800px;
  margin: 0 auto;
}

.workflow-step {
  display: flex;
  gap: var(--spacing-xl);
  padding: var(--spacing-2xl);
  align-items: flex-start;
}

.step-number {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-heading);
  color: var(--color-primary);
  font-family: var(--font-family-heading);
  background: color-mix(in srgb, var(--color-primary) 10%, transparent);
  padding: var(--spacing-md);
  border-radius: var(--border-radius-md);
  min-width: 60px;
  text-align: center;
}

.step-content h3 {
  font-size: var(--font-size-lg);
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
  margin-bottom: var(--spacing-md);
  font-family: var(--font-family-heading);
}

.step-content p {
  color: var(--color-on-surface-secondary);
  margin-bottom: var(--spacing-lg);
  line-height: var(--line-height-body);
}

.step-features {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-sm);
}

.step-feature {
  background: var(--color-backplate);
  padding: var(--spacing-xs) var(--spacing-md);
  border-radius: var(--border-radius-full);
  font-size: var(--font-size-sm);
  border: 1px solid var(--color-border);
}

/* Языки */
.languages-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: var(--spacing-xl);
  margin-bottom: var(--spacing-2xl);
}

.language-card {
  display: flex;
  gap: var(--spacing-lg);
  padding: var(--spacing-xl);
  align-items: flex-start;
}

.lang-icon {
  font-size: var(--font-size-2xl);
  flex-shrink: 0;
}

.lang-info h3 {
  font-size: var(--font-size-lg);
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
  margin-bottom: var(--spacing-sm);
  font-family: var(--font-family-heading);
}

.lang-info p {
  color: var(--color-on-surface-secondary);
  margin-bottom: var(--spacing-md);
  line-height: var(--line-height-body);
}

.lang-features {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-xs);
}

.lang-feature {
  background: color-mix(in srgb, var(--color-primary) 10%, transparent);
  color: var(--color-primary);
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius-full);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-heading);
}

.languages-cta {
  text-align: center;
}

.link {
  color: var(--color-primary);
  text-decoration: none;
}

.link:hover {
  text-decoration: underline;
}

/* CTA секция */
.cta-section {
  background: linear-gradient(135deg,
  color-mix(in srgb, var(--color-primary) 8%, transparent) 0%,
  color-mix(in srgb, var(--color-secondary) 12%, transparent) 100%);
}

.cta-content {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: var(--spacing-2xl);
  padding: var(--spacing-3xl);
  align-items: center;
  text-align: center;
}

.cta-text h2 {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-heading);
  color: var(--color-on-surface);
  margin-bottom: var(--spacing-lg);
  font-family: var(--font-family-heading);
}

.cta-text p {
  font-size: var(--font-size-lg);
  color: var(--color-on-surface-secondary);
  margin-bottom: var(--spacing-xl);
  line-height: var(--line-height-body);
}

.cta-stats {
  display: flex;
  justify-content: center;
  gap: var(--spacing-2xl);
}

.cta-stat {
  display: flex;
  flex-direction: column;
}

.cta-stat strong {
  font-size: var(--font-size-xl);
  font-weight: var(--font-weight-heading);
  color: var(--color-primary);
  font-family: var(--font-family-heading);
}

.cta-stat span {
  font-size: var(--font-size-sm);
  color: var(--color-on-surface-secondary);
}

.cta-actions {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

/* Адаптивность */
@media (max-width: 1024px) {
  .hero-content {
    grid-template-columns: 1fr;
    text-align: center;
  }

  .hero-description {
    max-width: 100%;
  }

  .hero-stats {
    justify-content: center;
  }

  .cta-content {
    grid-template-columns: 1fr;
    gap: var(--spacing-xl);
  }
}

@media (max-width: 768px) {
  .hero-title {
    font-size: var(--font-size-2xl);
  }

  .hero-actions {
    flex-direction: column;
    align-items: center;
  }

  .hero-stats {
    flex-direction: column;
    gap: var(--spacing-lg);
  }

  .features-grid {
    grid-template-columns: 1fr;
  }

  .workflow-step {
    flex-direction: column;
    text-align: center;
    gap: var(--spacing-lg);
  }

  .cta-stats {
    flex-direction: column;
    gap: var(--spacing-lg);
  }

  .floating-card {
    position: relative;
    margin-bottom: var(--spacing-md);
    animation: none;
  }

  .floating-elements {
    position: relative;
    margin-top: var(--spacing-xl);
  }
}

@media (max-width: 480px) {
  .feature-card,
  .language-card,
  .workflow-step {
    padding: var(--spacing-lg);
  }

  .cta-content {
    padding: var(--spacing-xl);
  }

  .section-title {
    font-size: var(--font-size-xl);
  }
}
</style>