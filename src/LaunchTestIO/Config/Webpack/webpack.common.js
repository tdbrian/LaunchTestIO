var webpack = require('webpack');
var ExtractTextPlugin = require('extract-text-webpack-plugin');
var helpers = require('../Utils/helpers');

module.exports = {
    entry: {
        'polyfills': './App/_core/polyfills.config.ts',
        'vendor': './App/_core/vendor.config.ts',
        'app': './App/main.config.ts'
    },

    resolve: { extensions: ['', '.ts', '.js'] },

    module: {
        loaders: [
          { test: /\.ts$/, loaders: ['awesome-typescript-loader', 'angular2-template-loader'] },
          { test: /\.html$/, loader: 'html' },
          { test: /\.(png|jpe?g|gif|svg|woff|woff2|ttf|eot|ico)$/, loader: 'file?name=[name].[hash].[ext]' },
          { test: /\.css$/, exclude: helpers.root('App'), loader: ExtractTextPlugin.extract('style', 'css?sourceMap') },
          { test: /\.css$/, include: helpers.root('App'), loader: 'raw' }
        ]
    },

    plugins: [
      new webpack.optimize.CommonsChunkPlugin({ name: ['app', 'vendor', 'polyfills'] }),
    ]
};
