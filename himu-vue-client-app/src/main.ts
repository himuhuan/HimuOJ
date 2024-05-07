import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './routers'
import { createPinia } from 'pinia'
import 'vfonts/Inter.css'
import 'vfonts/FiraCode.css'
import PersistedStatePlugin from 'pinia-plugin-persistedstate';
// @ts-ignore
import "mathjax/es5/tex-mml-chtml";

import JsonWorker from 'monaco-editor/esm/vs/language/json/json.worker?worker';

self.MonacoEnvironment = {
    getWorker() {
        return new JsonWorker();
    },
};

const app = createApp(App)

const pinia = createPinia()
pinia.use(PersistedStatePlugin);

app.use(pinia)
app.use(router)
app.mount('#app')