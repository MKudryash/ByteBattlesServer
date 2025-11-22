import Vue from 'vue'
import Router from 'vue-router'
import Meta from 'vue-meta'

import TaskTemplateBuilder from './views/task-template-builder'
import Home from './views/home'
import Auth from './views/auth'
import NotFound from './views/not-found'
import UserStats from './views/userstats.vue'
import StudentProfile from './views/student-profile.vue'
import UserProfile from './views/user-profile.vue'
import TaskList from './views/task-list.vue'
import TaskEdit from './views/task-edit.vue'
import './style.css'

Vue.use(Router)
Vue.use(Meta)

const router = new Router({
    mode: 'history',
    routes: [
        {
            name: 'Task-Template-Builder',
            path: '/task-template-builder',
            component: TaskTemplateBuilder,
            meta: {
                requiresAuth: true,
                requiresTeacher: true // Только для преподавателей
            }
        },
        {
            name: 'Home',
            path: '/',
            component: Home,
        },
        {
            name: 'Auth',
            path: '/auth',
            component: Auth,
            meta: {
                requiresGuest: true // Доступно только для неавторизованных
            }
        },
        {
            name: 'UserStats',
            path: '/students',
            component: UserStats,
            meta: {
                requiresAuth: true,
                requiresTeacher: true
            }
        },
        {
            name: 'StudentProfile',
            path: '/profile/:userId?',
            component: StudentProfile,
            meta: {
                requiresAuth: true,
                requiresTeacher: true
            },
            props: true
        },
        {
            name: 'UserProfile',
            path: '/me/:id?',
            component: UserProfile,
            meta: {
                requiresAuth: true,
                requiresTeacher: true
            },
            props: true
        },
        {
            name: 'TaskList',
            path: '/tasks',
            component: TaskList,
            meta: {
                requiresAuth: true,
                requiresTeacher: true
            }
        },
        {
            name: 'TaskEdit',
            path: '/tasks/:taskId/edit',
            component: TaskEdit,
            meta: {
                requiresAuth: true,
                requiresTeacher: true
            },
            props: true
        },
        {
            name: '404 - Not Found',
            path: '**',
            component: NotFound,
            fallback: true,
        },
    ],
})


function isAuthenticated() {
    const userData = localStorage.getItem('user')
    if (!userData) return false

    try {
        const parsedData = JSON.parse(userData)
        return !!(parsedData.user || parsedData)
    } catch (error) {
        console.error('Error parsing user data:', error)
        return false
    }
}

function isTeacher() {
    const userData = localStorage.getItem('user')
    if (!userData) return false

    try {
        const parsedData = JSON.parse(userData)
        const user = parsedData.user || parsedData

        // Обрабатываем разные форматы роли
        if (!user.role) return false
        const roleName = user.role.name || user.role
        return roleName === 'teacher' || roleName === 'admin'
    } catch (error) {
        console.error('Error parsing user data:', error)
        return false
    }
}

router.beforeEach((to, from, next) => {
    const requiresAuth = to.matched.some(record => record.meta.requiresAuth)
    const requiresTeacher = to.matched.some(record => record.meta.requiresTeacher)
    const requiresGuest = to.matched.some(record => record.meta.requiresGuest)

    const authenticated = isAuthenticated()
    const teacher = isTeacher()

    // Если маршрут требует авторизации, а пользователь не авторизован
    if (requiresAuth && !authenticated) {
        next({
            path: '/auth',
            query: {
                redirect: to.fullPath,
                message: 'Для доступа к этой странице необходимо войти в систему'
            }
        })
        return
    }

    // Если маршрут требует роль преподавателя, а у пользователя нет прав
    if (requiresTeacher && !teacher) {
        next({
            path: '/',
            query: {
                error: 'Недостаточно прав для доступа к этой странице'
            }
        })
        return
    }

    // Если маршрут доступен только для гостей (неавторизованных)
    if (requiresGuest && authenticated) {
        next({
            path: '/',
            query: {
                message: 'Вы уже авторизованы'
            }
        })
        return
    }

    next()
})

// Обработка ошибок навигации
router.onError((error) => {
    console.error('Navigation error:', error)

    if (error.message && error.message.includes('Failed to fetch dynamically imported module')) {
        // Обработка ошибок загрузки компонентов
        router.push('/')
    }
})

export default router