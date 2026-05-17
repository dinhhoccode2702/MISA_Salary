import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'

/* Global Styles */
import './assets/styles/main.css'
import './assets/styles/utilities.css'

/* DevExtreme Styles (Optional if using CDN, but better local) */
import 'devextreme/dist/css/dx.light.css'

const app = createApp(App)

app.use(createPinia())
app.use(router)

app.mount('#app')
