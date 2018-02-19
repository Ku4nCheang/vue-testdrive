const path = require('path')
const webpack = require('webpack')
const ExtractTextPlugin = require('extract-text-webpack-plugin')
const UglifyJSPlugin = require('uglifyjs-webpack-plugin')

module.exports = (env) => {
    const isDevBuild = !(env && env.prod)
    const extractCSS = new ExtractTextPlugin('vendor.css')

    return [{
        stats: { modules: false },
        resolve: { extensions: [ '.js' ] },
        devtool: 'source-map',
        entry: {
            vendor: [
                // uncomment following to support IE11
                'event-source-polyfill',
                // 'babel-polyfill',
                'axios',
                'purecss/build/pure.css',
                'vue',
                'vue-router',
                'vuex',
                'vue-class-component',
                'vue-property-decorator'
            ]
        },
        module: {
            exprContextRegExp: /$^/,
            exprContextCritical: false,
            rules: [
                { test: /\.css(\?|$)/, use: extractCSS.extract({ use: 'css-loader?minimize' }) },
                { test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/, use: 'url-loader?limit=100000' }
            ]
        },
        output: {
            path: path.join(__dirname, 'wwwroot', 'dist'),
            publicPath: 'dist/',
            filename: '[name].js',
            library: '[name]_[hash]'
        },
        plugins: [
            extractCSS,
            new webpack.ProvidePlugin({ $: 'jquery', jQuery: 'jquery' }), // Maps these identifiers to the jQuery package (because Bootstrap expects it to be a global variable)
            new webpack.DefinePlugin({
                'process.env': {
                    NODE_ENV: JSON.stringify(isDevBuild ? 'development' : 'production')
                }
            }),
            new webpack.DllPlugin({
                path: path.join(__dirname, 'wwwroot', 'dist', '[name]-manifest.json'),
                name: '[name]_[hash]'
            })
        ].concat(isDevBuild ? [] : [
            new UglifyJSPlugin({
                compress: {
                    warnings: false
                }
            })
        ])
    }]
}
