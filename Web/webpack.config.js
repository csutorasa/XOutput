const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const HtmlWebpackInlineSourcePlugin = require('html-webpack-inline-source-plugin');
const fs = require('fs');

const net452dir = path.resolve(__dirname, '../XOutput/bin/Release/net452/web');
const nercoreapp31dir = path.resolve(__dirname, '../XOutput/bin/Release/netcoreapp3.1/web');

fs.mkdirSync(net452dir, { recursive: true });
fs.mkdirSync(nercoreapp31dir, { recursive: true });

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
            }
        ],
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js'],
    },
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: '[name].js',
    },
    plugins: [
        new HtmlWebpackPlugin({
            title: 'XOutput'
        }),
        {
            apply: (compiler) => {
                compiler.hooks.afterEmit.tap('AfterEmitPlugin', (compilation) => {
                    fs.createReadStream('dist/index.html').pipe(fs.createWriteStream(net452dir + '/index.html'));
                    fs.createReadStream('dist/index.js').pipe(fs.createWriteStream(net452dir + '/index.js'));
                });
            }
        }
    ],
};