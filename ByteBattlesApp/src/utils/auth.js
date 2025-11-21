// Утилиты для работы с аутентификацией
export const authUtils = {
    // Получение данных пользователя
    getUser() {
        const userData = localStorage.getItem('user')
        if (!userData) return null

        try {
            const parsedData = JSON.parse(userData)
            return parsedData.user || parsedData
        } catch (error) {
            console.error('Error parsing user data:', error)
            this.clearAuth()
            return null
        }
    },

    // Проверка авторизации
    isAuthenticated() {
        return !!this.getUser()
    },

    // Проверка роли преподавателя
    isTeacher() {
        const user = this.getUser()
        return user && (user.role === 'teacher' || user.role === 'admin')
    },

    // Проверка роли студента
    isStudent() {
        const user = this.getUser()
        return user && user.role === 'student'
    },

    // Очистка данных аутентификации
    clearAuth() {
        localStorage.removeItem('user')
        localStorage.removeItem('accessToken')
        localStorage.removeItem('refreshToken')
        localStorage.removeItem('rememberMe')
    },

    // Сохранение данных пользователя
    setUser(userData) {
        try {
            localStorage.setItem('user', JSON.stringify(userData))
            return true
        } catch (error) {
            console.error('Error saving user data:', error)
            return false
        }
    },

    // Получение токена
    getToken() {
        return localStorage.getItem('accessToken')
    }
}