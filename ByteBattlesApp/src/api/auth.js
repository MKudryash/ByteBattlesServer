// Вместо полного URL используйте относительный путь
export const authAPI = {
    login: async (userData) => {
        try {
            const response = await fetch(`/api/auth/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'accept': 'application/json'
                },
                body: JSON.stringify(userData)
            })

            console.log('Login response status:', response.status);
            console.log('Login response headers:', response.headers);

            return response;
        } catch (error) {
            console.error('Login API error:', error);
            throw error;
        }
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