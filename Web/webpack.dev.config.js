const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');

module.exports = {
    entry: {
        'index': './src/index.ts',
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/,
            },
            {
                test: /\.s[ac]ss$/i,
                use: [
                    'style-loader',
                    'css-loader',
                    'sass-loader',
                ],
            },
            {
                test: /\.html$/,
                use: 'raw-loader'
            },
            {
                test: /\.css$/,
                use: 'css-loader'
            },
            {
                test: /\.(woff(2)?|ttf|eot|svg)(\?v=\d+\.\d+\.\d+)?$/,
                loader: 'raw-loader',
            }
        ],
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js'],
    },
    output: {
        path: path.resolve(__dirname, 'webapp'),
        filename: '[name].js',
    },
    plugins: [
        new HtmlWebpackPlugin({
            title: 'XOutput'
        })
    ],
    optimization: {
        minimize: false
    },
    devServer: {
        port: 8080,
        contentBase: path.join(__dirname, 'webapp'),
        proxy: {
            '/api': 'http://localhost:8000',
            '/ws': {
                target: 'ws://localhost:8000',
                ws: true
            }
        }
    },
};