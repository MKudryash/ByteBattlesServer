// API client для работы с Task Service
const API_BASE_URL = '/api'; // Используем относительные пути

// Общая функция для выполнения запросов
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
        // Используем относительный путь - nginx проксирует на правильный сервер
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

        const data = await response.json();
        console.log('Response data:', data);
        return data;

    } catch (error) {
        console.error('API request failed for URL:', url, error);
        throw error;
    }
}

// Language API - ИСПРАВЛЕННЫЕ URL (убрали дублирование /api)
export const languageAPI = {
    // Получить язык по идентификатору
    getById: async (languageId) => {
        return makeRequest(`/api/language/${languageId}`);
    },


    // Удалить язык программирования
    delete: async (languageId) => {
        return makeRequest(`/api/language/${languageId}`, {
            method: 'DELETE'
        });
    },

    // Создать новый язык программирования
    create: async (createLanguageDto) => {
        return makeRequest('/api/language', {
            method: 'POST',
            body: createLanguageDto
        });
    },

    // Обновить язык программирования
    update: async (updateLanguageDto) => {
        return makeRequest('/api/language', {
            method: 'PUT',
            body: updateLanguageDto
        });
    },

    // Получить список языков с пагинацией
    getAllPaged: async (page, pageSize, searchTerm = '') => {
        const params = new URLSearchParams({
            Page: page.toString(),
            PageSize: pageSize.toString(),
            ...(searchTerm && { SearchTerm: searchTerm })
        });

        return makeRequest(`/api/language/search-paged?${params}`);
    },

    // Получить список языков - ИСПРАВЛЕННЫЙ URL
    getAll: async (searchTerm = '') => {
        const params = new URLSearchParams();
        if (searchTerm) {
            params.append('SearchTerm', searchTerm);
        }

        const queryString = params.toString();
        // Теперь URL будет /api/language/search?SearchTerm=...
        return makeRequest(`/api/language/search${queryString ? `?${queryString}` : ''}`);
    },

    // Добавить библиотеки для языка
    createLibraries: async (languageId, createLibrariesDto) => {
        return makeRequest(`/api/language/library/${languageId}`, {
            method: 'POST',
            body: createLibrariesDto
        });
    },

    // Получить библиотеки для языка
    getLibraries: async (languageId) => {
        return makeRequest(`/api/language/library/${languageId}`);
    },

    // Обновить библиотеку
    updateLibrary: async (libraryId, updateLibraryDto) => {
        return makeRequest(`/api/language/library/${libraryId}`, {
            method: 'PUT',
            body: updateLibraryDto
        });
    },

    // Удалить библиотеку
    deleteLibrary: async (libraryId) => {
        return makeRequest(`/api/language/library/${libraryId}`, {
            method: 'DELETE'
        });
    }
};

// Task API - также добавляем /api ко всем URL
export const taskAPI = {
    // Получить задачу по идентификатору
    getById: async (taskId) => {
        return makeRequest(`/api/task/${taskId}`);
    },

    // Удалить задачу
    delete: async (taskId) => {
        return makeRequest(`/api/task/${taskId}`, {
            method: 'DELETE'
        });
    },

    // Создать новую задачу
    create: async (createTaskDto) => {
        return makeRequest('/api/task', {
            method: 'POST',
            body: createTaskDto
        });
    },

    // Обновить задачу
    update: async (updateTaskDto) => {
        return makeRequest('/api/task', {
            method: 'PUT',
            body: updateTaskDto
        });
    },

    // Получить список задач с пагинацией
    getAllPaged: async (page, pageSize, filters = {}) => {
        const params = new URLSearchParams({
            Page: page.toString(),
            PageSize: pageSize.toString(),
            ...(filters.searchTerm && { SearchTerm: filters.searchTerm }),
            ...(filters.difficulty && { Difficulty: filters.difficulty }),
            ...(filters.languageId && { LanguageId: filters.languageId })
        });

        return makeRequest(`/api/task/search-paged?${params}`);
    },

    // Получить список задач
    getAll: async (filters = {}) => {
        const params = new URLSearchParams();
        if (filters.searchTerm) params.append('SearchTerm', filters.searchTerm);
        if (filters.difficulty) params.append('Difficulty', filters.difficulty);
        if (filters.languageId) params.append('LanguageId', filters.languageId);

        const queryString = params.toString();
        return makeRequest(`/api/task/search${queryString ? `?${queryString}` : ''}`);
    },

    // Добавить тестовые случаи для задачи
    createTestCases: async (taskId, createTestCasesDto) => {
        return makeRequest(`/api/task/testCases/${taskId}`, {
            method: 'POST',
            body: createTestCasesDto
        });
    },

    // Получить тестовые случаи для задачи
    getTestCases: async (taskId) => {
        return makeRequest(`/api/task/testCases/${taskId}`);
    },
    getCount: async () => {
        return makeRequest(`/api/task/count`);
    },
    // Обновить тестовый случай
    updateTestCase: async (testCaseId, updateTestCaseDto) => {
        return makeRequest(`/api/task/testCases/${testCaseId}`, {
            method: 'PUT',
            body: updateTestCaseDto
        });
    },

    // Удалить тестовый случай
    deleteTestCase: async (testCaseId) => {
        return makeRequest(`/api/task/testCases/${testCaseId}`, {
            method: 'DELETE'
        });
    }
};


// Константы и вспомогательные функции остаются без изменений
export const DIFFICULTY = {
    EASY: 1,
    MEDIUM: 2,
    HARD: 3
};

export const apiHelpers = {
    toQueryString: (params) => {
        const searchParams = new URLSearchParams();
        Object.keys(params).forEach(key => {
            if (params[key] !== null && params[key] !== undefined) {
                searchParams.append(key, params[key]);
            }
        });
        return searchParams.toString();
    },

    setAuthToken: (token) => {
        localStorage.setItem('authToken', token);
    },

    removeAuthToken: () => {
        localStorage.removeItem('authToken');
    },

    hasAuthToken: () => {
        return !!localStorage.getItem('authToken');
    }
};

export default {
    language: languageAPI,
    task: taskAPI,

    DIFFICULTY,
    helpers: apiHelpers
};