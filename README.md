# Integration Testing - Doing it the right way

A presentation about effective integration testing practices built with reveal.js.

## Live Presentation

The presentation is automatically deployed to GitHub Pages: [View Live](https://m4cx.github.io/integration-testing-done-right/)

### PR Previews

Pull request previews are automatically deployed when you open a PR. Each PR gets its own preview URL at:
`https://m4cx.github.io/integration-testing-done-right/pr-{number}/`

When you open a pull request, the GitHub Actions bot will comment with a direct link to your PR preview.

## Setup

1. Install dependencies:
   ```bash
   npm install
   ```

2. Start the presentation server:
   ```bash
   npm start
   ```

3. Open your browser and navigate to `http://localhost:3000`

## Development

To start the development server with auto-reload:
```bash
npm run dev
```

## Building for Deployment

To build the presentation for production deployment:
```bash
npm run build
```

To build and test locally:
```bash
npm run build:serve
```

The built files will be in the `dist/` directory, ready for deployment to any static hosting service.

## Deployment

The presentation is automatically deployed to GitHub Pages when changes are pushed to the main branch. The GitHub Actions workflow:

1. Installs dependencies
2. Builds the presentation using `npm run build`
3. Deploys the built files to GitHub Pages

### PR Previews

Pull requests also trigger automatic deployments for preview purposes:

1. Each PR gets deployed to a subdirectory: `/pr-{number}/`
2. The main branch version remains at the root URL
3. PR previews are automatically updated when new commits are pushed
4. A comment with the preview link is added to each PR

This allows reviewers to see changes without cloning the repository locally.

## Features

- Modern, responsive design with custom theming
- Smooth transitions and animations
- Code syntax highlighting
- Speaker notes support
- Keyboard navigation
- Mobile-friendly layout

## Navigation

- **Arrow keys**: Navigate between slides
- **Space**: Next slide
- **Esc**: Overview mode
- **S**: Speaker notes view
- **F**: Fullscreen mode

## Customization

The presentation uses a custom theme located in `css/custom-theme.css`. You can modify colors, fonts, and styling by editing this file.

## Built with

- [reveal.js](https://revealjs.com/) - HTML presentation framework
- Custom CSS theming with modern design principles
- Google Fonts (Inter & JetBrains Mono)

## License

MIT - See LICENSE file for details.