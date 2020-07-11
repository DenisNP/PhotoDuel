import Vue from 'vue';
import Framework7 from 'framework7/framework7-lite.esm.bundle';
import Framework7Vue from 'framework7-vue/framework7-vue.esm.bundle';
import 'framework7/css/framework7.bundle.css';
import VueCroppie from 'vue-croppie';
import 'croppie/croppie.css';
import App from './App.vue';
import store from './store';

Framework7.use(Framework7Vue);
Vue.use(VueCroppie);

Vue.config.productionTip = false;

export default new Vue({
    store,
    render: (h) => h(App),
}).$mount('#app');
