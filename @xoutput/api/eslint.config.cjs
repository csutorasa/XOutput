const react = require('eslint-plugin-react');
const globals = require('globals');

module.exports = [
  {
    files: ['src/**/*.{js,jsx,mjs,cjs,ts,tsx}'],
    languageOptions: {
      globals: {
        ...globals.browser,
      },
    },
  }
];