// create a install plugin for registering directive for Vue
import Vue from 'vue'

export default {
    install: () => {
        Vue.directive('focus', require('./focus').default)
    }
}
