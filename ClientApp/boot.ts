// import 'babel-polyfill'
import './static/site.css'
import Vue from 'vue'
import './hooks' // This must be imported before any component

Vue.config.devtools = process.env['NODE_ENV'] === 'development'
Vue.config.ignoredElements = [
    /ion-.*/
]

Vue.use(require('./directives').default)
Vue.use(require('./components').default)

/* tslint:disable-next-line:no-unused-expression */
new Vue({
    el: '#app-root',
    template: '<app/>',
    store: require('./store').default,
    router: require('./router').default,
    render: h => h(require('./components/app/app.vue').default)
})
