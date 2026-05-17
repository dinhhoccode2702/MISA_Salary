import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/',
    redirect: '/salary-composition'
  },
  {
    path: '/salary-composition',
    name: 'SalaryCompositionList',
    component: () => import('../views/salary-composition/SalaryCompositionList.vue')
  },
  {
    path: '/salary-composition/add',
    name: 'SalaryCompositionAdd',
    component: () => import('../views/salary-composition/SalaryCompositionDetail.vue')
  },
  {
    path: '/salary-composition/:id',
    name: 'SalaryCompositionEdit',
    component: () => import('../views/salary-composition/SalaryCompositionDetail.vue'),
    props: true
  },
  {
    path: '/system-dictionary',
    name: 'SystemDictionary',
    component: () => import('../views/salary-composition/SystemDictionary.vue')
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
