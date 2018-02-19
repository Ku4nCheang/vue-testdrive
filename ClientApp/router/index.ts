import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

export default new Router({
    routes: [
        { path: '/', component: require('../components/home/home.vue').default },
        { path: '/fetchdata', component: require('../components/fetchdata/fetchdata.vue').default },
        { path: '/desk', component: require('../components/desk/desk.vue').default }
    ],
    mode: 'history'
})
