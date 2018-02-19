// Root state of the store
export default interface State {
    forecast: ForecastState
}

export interface WeatherForecast {
    dateFormatted: string
    temperatureC: number
    temperatureF: number
    summary: string
}

export interface RequestStatus {
    loading: boolean,
    failed: boolean,
    received: boolean,
    errorCode: string,
    errorMessage: string
}

export interface ForecastState {
    // Define properties
    forecastStatus: RequestStatus
    forecastData: WeatherForecast[]
}
