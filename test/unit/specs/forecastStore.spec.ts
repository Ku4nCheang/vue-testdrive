import Vue from 'vue'
import Vuex from 'vuex'
import { expect } from 'chai'
import { mount } from 'avoriaz'
import sinon from 'sinon'
import sinonStubPromise from 'sinon-stub-promise'
import FetchDataComponent from '@components/fetchdata/fetchdata'
import store from '@store/index'
import * as Forecast from '@store/modules/forecast'
import Axios, { AxiosResponse } from 'axios'

sinonStubPromise(sinon)

const mockResponse = (body: any = {}) => {
    return {
        data: body,
        status: 200,
        statusText: 'OK',
        headers: {},
        config: {},
        request: {}
    } as AxiosResponse<any>
}

describe('vuex: forecast module', () => {
    let stubFetch: sinon.SinonStub

    beforeEach(() => {
        // do something be each test
        // reset the state for test
        store.replaceState({
            forecast: {
                forecastData: [],
                forecastStatus: {
                    loading: false,
                    failed: false,
                    received: false,
                    errorCode: '',
                    errorMessage: ''
                }
            }
        })
        stubFetch = sinon.stub(Axios, 'get').returns(
            Promise.resolve(mockResponse([
                {
                    dateFormatted: '11/07/2017',
                    summary: 'Hot',
                    temperatureC: -10,
                    temperatureF: 15
                }
            ])
        ))
    })

    afterEach(() => {
        stubFetch.restore()
    })

    it('should get correct json forecast data via dispatch', async () => {
        await Forecast.dispatchRequestForecastData(store)
        expect(store.state.forecast.forecastData[0]).to.eql({
            dateFormatted: '11/07/2017',
            summary: 'Hot',
            temperatureC: -10,
            temperatureF: 15
        })

        expect(store.state.forecast.forecastStatus).to.eql({
            loading: false,
            failed: false,
            received: true,
            errorCode: '',
            errorMessage: ''
        })
    })

    it('should change correct request status via commit', () => {
        Forecast.commitForecastStatusWillStart(store)
        expect(store.state.forecast.forecastStatus).to.eql({
            loading: true,
            failed: false,
            received: false,
            errorCode: '',
            errorMessage: ''
        })

        Forecast.commitForecastStatusDidFail(store)
        expect(store.state.forecast.forecastStatus).to.eql({
            loading: false,
            failed: true,
            received: false,
            errorCode: '',
            errorMessage: ''
        })

        Forecast.commitClear(store)
        expect(store.state.forecast.forecastStatus).to.eql({
            loading: false,
            failed: false,
            received: false,
            errorCode: '',
            errorMessage: ''
        })
    })

    it('should get correct data via getter', async () => {
        // load the data from mock api
        await Forecast.dispatchRequestForecastData(store)

        const data = Forecast.readForecastData(store)
        expect(data).to.eql([{
            dateFormatted: '11/07/2017',
            summary: 'Hot',
            temperatureC: -10,
            temperatureF: 15
        }])

        const status = Forecast.readForecastStatus(store)
        expect(status).to.eql({
            loading: false,
            failed: false,
            received: true,
            errorCode: '',
            errorMessage: ''
        })

        const count = Forecast.readCountForecastData(store)
        expect(count).to.eql(1)

    })
})
