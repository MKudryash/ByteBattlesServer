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
export default new Router({
  mode: 'history',
  routes: [
    {
      name: 'Task-Template-Builder',
      path: '/task-template-builder',
      component: TaskTemplateBuilder,
    },
    {
      name: 'Home',
      path: '/',
      component: Home,
    },
    {
      name: 'Auth',
      path: '/auth',
      component: Auth
    },
      {
      name: 'UserStats',
      path: '/students',
      component: UserStats
    },
      {
          name: 'StudentProfile',
          path: '/profile/:userId?', // Вопросительный знак делает параметр опциональным
          component: StudentProfile,
          meta: {
              requiresAuth: true
          },
          props: true // Передавать параметры как props
      },
      {
          name: 'UserProfile',
          path: '/me/:id?', // Опциональный ID
          component: UserProfile,
          meta: {
              requiresAuth: true
          },
          props: true
      },
      {
          name: 'TaskList',
          path: '/tasks',
          component: TaskList,
          meta: {
              requiresAuth: true
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
