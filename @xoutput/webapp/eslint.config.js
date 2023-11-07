const react = require('eslint-plugin-react');
const globals = require('globals');

module.exports = [
  {
    files: ['src/**/*.{js,jsx,mjs,cjs,ts,tsx}'],
    plugins: {
      react,
    },
    languageOptions: {
      parserOptions: {
        ecmaFeatures: {
          jsx: true,
        },
      },
      globals: {
        ...globals.browser,
      },
    },
    rules: {
        "no-restricted-imports": [
            "error",
            {
                "paths": ["@mui/material", "@mui/icons-material"]
            }
        ],
        "react/prop-types": "off",
     },
  },
];