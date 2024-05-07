// vuex.d.ts
import { Store } from 'vuex'

declare module '@vue/runtime-core' {
    interface State {
        accessToken: string,
        userId: string,
        userName: string,
    }

    interface ComponentCustomProperties {
        $store: Store<State>
    }
}