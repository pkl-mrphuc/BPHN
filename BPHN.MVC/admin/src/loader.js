import ConfigAPI from '@/apis/ConfigAPI'
export async function loadConfig() {
    if (!window['LoadedConfig']) {
        window['LoadedConfig'] = true
        let res = await ConfigAPI.getConfigs('')
        if (res?.data?.success && res?.data?.data) {
            let lstConfig = res?.data?.data
            if (Array.isArray(lstConfig)) {
                for (let i = 0; i < lstConfig.length; i++) {
                    const item = lstConfig[i];
                    window[item.key] = item.value
                }
            }
        }
    }
}