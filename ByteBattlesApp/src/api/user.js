// API client для работы с User Profile Service
const API_BASE_URL = '/api';

// Общая функция для выполнения запросов (уже реализована, оставляем как есть)
async function makeRequest(url, options = {}) {
    const token = localStorage.getItem('accessToken');

    console.log('Making request to:', url);
    console.log('Token present:', !!token);

    const defaultOptions = {
        headers: {
            'Content-Type': 'application/json',
            'accept': '*/*',
            ...(token && { 'Authorization': `Bearer ${token}` }),
            ...options.headers
        }
    };

    const config = {
        ...defaultOptions,
        ...options,
        headers: {
            ...defaultOptions.headers,
            ...options.headers
        }
    };

    if (config.body && typeof config.body === 'object') {
        config.body = JSON.stringify(config.body);
    }

    try {
        const fullUrl = url.startsWith('/') ? url : `/${url}`;
        console.log('Full URL:', fullUrl);

        const response = await fetch(fullUrl, config);
        console.log('Response status:', response.status);

        if (!response.ok) {
            let errorMessage = `HTTP error! status: ${response.status}`;
            try {
                const errorData = await response.json();
                errorMessage = errorData.message || errorData.title || errorMessage;
                console.log('Error details:', errorData);
            } catch {
                const text = await response.text();
                console.log('Error response text:', text);
            }
            throw new Error(errorMessage);
        }

        // Для ответов без контента (например, 200 без тела)
        if (response.status === 200 && response.headers.get('content-length') === '0') {
            return null;
        }

        const data = await response.json();
        console.log('Response data:', data);
        return data;

    } catch (error) {
        console.error('API request failed for URL:', url, error);
        throw error;
    }
}

// User Profiles API
export const userProfilesAPI = {
    // Получить профиль текущего пользователя
    getMyProfile: async () => {
        return makeRequest('/api/user-profiles/me');
    },

    // Обновить профиль текущего пользователя
    updateMyProfile: async (updateProfileCommandDto) => {
        return makeRequest('/api/user-profiles/me', {
            method: 'PUT',
            body: updateProfileCommandDto
        });
    },

    // Обновить настройки текущего пользователя
    updateMySettings: async (updateSettingsCommandDto) => {
        return makeRequest('/api/user-profiles/me/settings', {
            method: 'PUT',
            body: updateSettingsCommandDto
        });
    },

    // Обновить статистику текущего пользователя
    updateMyStats: async (userStatsCommandDto) => {
        return makeRequest('/api/user-profiles/me/stats', {
            method: 'PUT',
            body: userStatsCommandDto
        });
    },

    // Получить профиль пользователя по ID
    getProfileById: async (profileId) => {
        return makeRequest(`/api/user-profiles/${profileId}`);
    },

    // Создать новый профиль пользователя
    createProfile: async (createUserProfileCommandDto) => {
        return makeRequest('/api/user-profiles', {
            method: 'POST',
            body: createUserProfileCommandDto
        });
    },

    // Получить таблицу лидеров
    getLeaderboard: async (countTop = 5) => {
        const params = new URLSearchParams({
            countTop: countTop.toString()
        });
        return makeRequest(`/api/user-profiles/leaderboard?${params}`);
    },

    // Поиск пользователей
    searchUsers: async (searchTerm = '', page = 1, pageSize = 10) => {
        const params = new URLSearchParams({
            Page: page.toString(),
            PageSize: pageSize.toString()
        });

        if (searchTerm) {
            params.append('SearchTerm', searchTerm);
        }

        return makeRequest(`/api/user-profiles/search?${params}`);
    },

    // Получить активность текущего пользователя
    getMyActivity: async (activitiesLimit = null, problemsLimit = null) => {
        const params = new URLSearchParams();
        if (activitiesLimit !== null) {
            params.append('activitiesLimit', activitiesLimit.toString());
        }
        if (problemsLimit !== null) {
            params.append('problemsLimit', problemsLimit.toString());
        }

        const queryString = params.toString();
        return makeRequest(`/api/user-profiles/me/activity${queryString ? `?${queryString}` : ''}`);
    },

    // Получить последние активности текущего пользователя
    getMyRecentActivities: async (limit = 50) => {
        const params = new URLSearchParams({
            limit: limit.toString()
        });
        return makeRequest(`/api/user-profiles/me/recent-activities?${params}`);
    },

    // Получить последние решенные задачи текущего пользователя
    getMyRecentProblems: async (limit = 20) => {
        const params = new URLSearchParams({
            limit: limit.toString()
        });
        return makeRequest(`/api/user-profiles/me/recent-problems?${params}`);
    },
};

// Вспомогательные функции и константы
export const userProfileHelpers = {
    // Преобразование DTO настроек в форму для редактирования
    settingsToForm: (settingsDto) => {
        return {
            emailNotifications: settingsDto.emailNotifications || false,
            battleInvitations: settingsDto.battleInvitations || false,
            achievementNotifications: settingsDto.achievementNotifications || false,
            theme: settingsDto.theme || 'light',
            codeEditorTheme: settingsDto.codeEditorTheme || 'vs-dark',
            preferredLanguage: settingsDto.preferredLanguage || 'javascript'
        };
    },

    // Преобразование DTO профиля в форму для редактирования
    profileToForm: (profileDto) => {
        return {
            userName: profileDto.userName || '',
            bio: profileDto.bio || '',
            country: profileDto.country || '',
            gitHubUrl: profileDto.gitHubUrl || '',
            linkedInUrl: profileDto.linkedInUrl || '',
            isPublic: profileDto.isPublic || false
        };
    },

    // Расчет прогресса до следующего уровня
    calculateLevelProgress: (totalExperience, experienceToNextLevel) => {
        if (!experienceToNextLevel || experienceToNextLevel === 0) return 100;
        const currentLevelExp = totalExperience - experienceToNextLevel;
        return Math.min(100, Math.max(0, (currentLevelExp / experienceToNextLevel) * 100));
    },

    // Форматирование статистики для отображения
    formatStats: (statsDto) => {
        return {
            totalProblemsSolved: statsDto.totalProblemsSolved || 0,
            totalBattles: statsDto.totalBattles || 0,
            wins: statsDto.wins || 0,
            losses: statsDto.losses || 0,
            draws: statsDto.draws || 0,
            currentStreak: statsDto.currentStreak || 0,
            maxStreak: statsDto.maxStreak || 0,
            totalExperience: statsDto.totalExperience || 0,
            winRate: statsDto.winRate ? Math.round(statsDto.winRate * 100) : 0,
            experienceToNextLevel: statsDto.experienceToNextLevel || 0
        };
    }
};

// Константы для тем и языков
export const USER_PROFILE_CONSTANTS = {
    THEMES: {
        LIGHT: 'light',
        DARK: 'dark',
        AUTO: 'auto'
    },

    CODE_EDITOR_THEMES: {
        VS: 'vs',
        VS_DARK: 'vs-dark',
        HIGH_CONTRAST: 'hc-black',
        HIGH_CONTRAST_LIGHT: 'hc-light'
    },


    DIFFICULTY: {
        EASY: 'Easy',
        MEDIUM: 'Medium',
        HARD: 'Hard'
    }
};

export default {
    userProfiles: userProfilesAPI,
    constants: USER_PROFILE_CONSTANTS,
    helpers: userProfileHelpers
};