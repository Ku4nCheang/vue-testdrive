import { getStoreAccessors } from 'vuex-typescript'
import State, { ForecastState, WeatherForecast, RequestStatus } from '../state'
import { ActionContext, Store } from 'vuex'

/* ---------------------------------------------------------------- */
// Declaration
//
// Declare your interface and type in this section
/* ---------------------------------------------------------------- */

const namespace = 'forecast'
type ForecastContext = ActionContext<ForecastState, State>

/* ---------------------------------------------------------------- */
// State
//
// The state of the store. Define your state in this section.
/* ---------------------------------------------------------------- */

const defaultForecastStatus = {
    loading: false,
    failed: false,
    received: false
}

const state: ForecastState = {
    // Define properties
    forecastStatus: {
        ...defaultForecastStatus
    },
    forecastData: []
}

/* ---------------------------------------------------------------- */
// Mutations
//
// The only way to change the state of the store by commiting
// mutations. Define your mutations in this section
/* ---------------------------------------------------------------- */

const mutations = {
    setClear (state: ForecastState) {
        state.forecastStatus = {
           ...defaultForecastStatus
        }
        state.forecastData = []
    },
    setForecastDataDidReceive (state: ForecastState, forecastData: WeatherForecast[]) {
        state.forecastStatus = {
            ...defaultForecastStatus,
            received: true
        }
        // set forecast data
        state.forecastData = forecastData
    },
    setForecastStatusWillStart (state: ForecastState) {
        state.forecastStatus = {
            ...defaultForecastStatus,
            loading: true
        }
        // reset forecast data when try to reload data
        state.forecastData = []
    },
    setForecastStatusDidFail (state: ForecastState) {
        state.forecastStatus = {
            ...defaultForecastStatus,
            failed: true
        }
        // reset forecast data when try to reload data
        state.forecastData = []
    }
}

/* ---------------------------------------------------------------- */
// Getters
//
// Sometimes we may need to compute derived state based on store state,
// for example filtering through a list of items and counting them.
// Define your getters in this section
/* ---------------------------------------------------------------- */

const getters = {
    getForecastData (state: ForecastState): WeatherForecast[] {
        return state.forecastData
    },
    getForecastStatus (state: ForecastState): RequestStatus {
        return state.forecastStatus
    },
    getCountForecastData (state: ForecastState): number {
        return state.forecastData.length
    }
}

/* ---------------------------------------------------------------- */
// Actions
//
// define all the actions with this section, action will perform when
// a dispatch signal is sent.
/* ---------------------------------------------------------------- */

const actions = {
    async resetForecastData (context: ForecastContext) {
        commitClear(context)
    },
    async requestForecastData (context: ForecastContext) {
        commitForecastStatusWillStart(context)
        // loading api to received data
        try {
            const response = await fetch('api/SampleData/WeatherForecasts')
            const data = await response.json() as WeatherForecast[]
            commitForecastDataDidReceive(context, data)
            return true
        } catch (reason) {
            commitForecastStatusDidFail(context)
        }

        return false
    }
}

/* ---------------------------------------------------------------- */
// Store Constructor
//
// Compose state, getters, actions and mutations as a store
/* ---------------------------------------------------------------- */

// fixed the ie problem: https://github.com/istrib/vuex-typescript/issues/13
const vset = [getters, actions, mutations]
vset.forEach((actions: any) => {
    Object.keys(actions).forEach((key) => {
        actions[key]['_vuexKey'] = key
    })
})

export const store = {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
}

export default store

/* ---------------------------------------------------------------- */
// Shorhand for access the store
//
// Define a shorthand to access actions, getters or mutations
// in the store.
/* ---------------------------------------------------------------- */

// Get the accessor to specified store
// The namespace must be equal to the name in the root store
const { commit, read, dispatch } = getStoreAccessors<ForecastState, State>(namespace)

/* ---------------------------------------------------------------- */

// Mutations shorthand
// export const commitSomething = commit(mutations.do_something)

// write your code here
export const commitClear = commit(mutations.setClear)
export const commitForecastStatusDidFail = commit(mutations.setForecastStatusDidFail)
export const commitForecastStatusWillStart = commit(mutations.setForecastStatusWillStart)
export const commitForecastDataDidReceive = commit(mutations.setForecastDataDidReceive)

/* ---------------------------------------------------------------- */

// Getters shorthand
// export const getSomething = read(getters.getSomethingInStore)

// write your code here
export const readForecastData = read(getters.getForecastData)
export const readForecastStatus = read(getters.getForecastStatus)
export const readCountForecastData = read(getters.getCountForecastData)

/* ---------------------------------------------------------------- */

// Actions shorthand
// export const dispatchSomething = dispatch(actions.doSomething)

// write your code here
export const dispatchRequestForecastData = dispatch(actions.requestForecastData)

/* ---------------------------------------------------------------- */
