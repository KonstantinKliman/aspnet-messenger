/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './wwwroot/index.html',
    './**/*.{razor,html}',
    '../Messenger.Client.Shared/**/*.{razor,html}',
  ],
  theme: {
    extend: {},
  },
  plugins: [
    require('@tailwindcss/forms'),
  ],
}