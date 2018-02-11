<template>
    <div class="pure-pd-10">
        <h1>Weather forecast</h1>
        <p>This component demonstrates fetching data from the server.</p>
        <div class="pure-mg-10-0">
            <button class="pure-button" @click="clear">clear</button>
            <button class="pure-button" @click="reload">reload</button>
            <button class="pure-button" @click="failed">failed</button>
            <button class="pure-button" @click="loading">loading</button>
        </div>
        <table v-if="forecastStatus.received" class="pure-table pure-table-bordered">
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Temp. (C)</th>
                    <th>Temp. (F)</th>
                    <th>Summary</th>
                </tr>
            </thead>
            <tbody>
                 <tr v-for="(item, index) in forecasts" :key="index">
                    <td>{{ item.dateFormatted }}</td>
                    <td>{{ item.temperatureC }}</td>
                    <td>{{ item.temperatureF }}</td>
                    <td>{{ item.summary }}</td>
                </tr>
                <tr><td colspan="4">Total Count: {{countForecasts}}</td></tr>
                <tr><td colspan="4">Time spent: {{requestTimeSpent}}</td></tr>
            </tbody>
        </table>
        
        <div v-if="forecastStatus.loading">
            <h3>Loading...</h3>
        </div>
        <div v-if="forecastStatus.failed">
            <h3>Oops, seems like there is something wrong</h3>
            <p>reason: {{failedReason}}</p>
        </div>
        <p>Request Status:</p>
        <pre>{{forecastStatus}}</pre>
    </div>
</template>

<script src="./fetchdata.ts"></script>
