import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import * as Forecast from '../../store/modules/forecast'

@Component
export default class FetchDataComponent extends Vue {
    timeSpent: number = 0
    failedReason: string = ''

    get forecasts () {
        return Forecast.readForecastData(this.$store)
    }

    get countForecasts () {
        return Forecast.readCountForecastData(this.$store)
    }

    get forecastStatus () {
        return Forecast.readForecastStatus(this.$store)
    }

    get requestTimeSpent (): string {
        return `${this.timeSpent}ms`
    }

    mounted () {
        this.reload()
    }

    beforeDestroy () {
        this.clear()
    }

    clear () {
        Forecast.commitClear(this.$store)
    }

    async reload () {
        let rt = Date.now()
        const result = await await Forecast.dispatchRequestForecastData(this.$store)
        if (result) {
            this.timeSpent = (Date.now() - rt)
        } else {
            this.failedReason = 'Could not get data from remote server'
        }
    }

    loading () {
        Forecast.commitForecastStatusWillStart(this.$store)
    }

    failed () {
        Forecast.commitForecastStatusDidFail(this.$store)
        this.failedReason = ':) you just set failed'
    }

 }
