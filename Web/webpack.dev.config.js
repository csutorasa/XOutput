const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const fs = require('fs');

const nercoreapp31dir = path.resolve(__dirname, '../XOutput.App/bin/Debug/netcoreapp3.1/webapp');

fs.mkdirSync(nercoreapp31dir, { recursive: true });

function copyFile(file) {
    fs.createReadStream(path.join('webapp', file)).pipe(fs.createWriteStream(path.join(nercoreapp31dir, file)));
}

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
        path: path.resolve(__dirname, 'webapp'),
        filename: '[name].js',
    },
    plugins: [
        new HtmlWebpackPlugin({
            title: 'XOutput'
        }),
        {
            apply: (compiler) => {
                compiler.hooks.afterEmit.tap('AfterEmitPlugin', (compilation) => {
                    copyFile('index.html');
                    copyFile('index.js');
                });
            }
        }
    ],
    optimization: {
        minimize: false
    },
};