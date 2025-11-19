// Вместо полного URL используйте относительный путь
export const authAPI = {
    login: async (userData) => {
        const response = await fetch(`/api/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'accept': '*/*'
            },
            body: JSON.stringify(userData)
        })
        return response
    },

    register: async (userData) => {
        const response = await fetch(`/api/auth/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'accept': '*/*'
            },
            body: JSON.stringify(userData)
        })
        return response
    }
}