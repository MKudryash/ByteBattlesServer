import Vue from 'vue'
import Router from 'vue-router'
import Meta from 'vue-meta'

import TaskTemplateBuilder from './views/task-template-builder'
import Home from './views/home'
import Auth from './views/auth'
import NotFound from './views/not-found'
import UserStats from './views/userstats.vue'
import StudentProfile from './views/profileuser.vue'
import TaskList from './views/task-list.vue' // Список задач
import TaskView from './views/TaskView.vue' // Просмотр задачи
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
          name: 'UserProfile',
          path: '/profile/:userId?', // Вопросительный знак делает параметр опциональным
          component: StudentProfile,
          meta: {
              requiresAuth: true
          },
          props: true // Передавать параметры как props
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
          name: 'TaskView',
          path: '/tasks/:taskId',
          component: TaskView,
          meta: {
              requiresAuth: true
          },
          props: true
      },
      {
          name: 'TaskEdit',
          path: '/tasks/:taskId/edit',
          component: TaskEdit,
          meta: {
              requiresAuth: true,
              requiresTeacher: true // Если нужно ограничить доступ
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
