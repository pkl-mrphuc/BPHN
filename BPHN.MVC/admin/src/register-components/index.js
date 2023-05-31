import FootballFieldDialog from '@/components/dialogs/BPHNFootballFieldDialog.vue'
const components = [FootballFieldDialog]
export default {
    install(Vue) {
        components.forEach((component) => {
            Vue.use(component.name, component)
        })
    }
}