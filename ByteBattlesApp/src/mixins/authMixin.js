import { authUtils } from '@/utils/auth'

export const authMixin = {
    computed: {
        currentUser() {
            return authUtils.getUser()
        },
        isAuthenticated() {
            return authUtils.isAuthenticated()
        },
        isTeacher() {
            return authUtils.isTeacher()
        },
        isStudent() {
            return authUtils.isStudent()
        }
    },
    methods: {
        requireAuth() {
            if (!this.isAuthenticated) {
                this.$router.push({
                    path: '/auth',
                    query: {
                        redirect: this.$route.fullPath,
                        message: 'Для доступа к этой странице необходимо войти в систему'
                    }
                })
                return false
            }
            return true
        },

        requireTeacher() {
            if (!this.requireAuth()) return false

            if (!this.isTeacher) {
                this.$router.push({
                    path: '/',
                    query: {
                        error: 'Недостаточно прав для выполнения этого действия'
                    }
                })
                return false
            }
            return true
        }
    }
}