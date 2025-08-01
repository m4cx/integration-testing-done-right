# Integration Testing - Doing it the right way

A presentation about effective integration testing practices built with reveal.js.

## Live Presentation

The presentation is automatically deployed to GitHub Pages: [View Live](https://m4cx.github.io/integration-testing-done-right/)

## Project Structure

```
├── src/presentation/           # Presentation source files
│   ├── index.html             # Main presentation file
│   └── css/                   # Custom styles and themes
├── examples/                  # Integration testing examples (for future development)
├── build.js                   # Build script for deployment
├── dist/                      # Built presentation (generated)
└── package.json              # Project configuration
```

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

The presentation is automatically deployed to GitHub Pages with two deployment modes:

### Prerequisites for PR Previews

For PR preview deployments to work, you need to set up a Personal Access Token (PAT):

1. Go to GitHub Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Generate a new token with the following permissions:
   - `repo` (Full control of private repositories)
   - `workflow` (Update GitHub Action workflows)
3. In your repository, go to Settings → Secrets and variables → Actions
4. Add a new repository secret named `GH_PAT` with your token value

This token allows the GitHub Actions workflow to push to the `gh-pages` branch for PR previews.

### Production Deployment (Main Branch)
When changes are pushed to the main branch, the GitHub Actions workflow:
1. Installs dependencies
2. Builds the presentation using `npm run build`
3. Deploys the built files to the root of GitHub Pages

### PR Preview Deployments
When a pull request is opened or updated, the workflow:
1. Builds the PR version of the presentation
2. Deploys it to a subdirectory: `https://m4cx.github.io/integration-testing-done-right/pr-{PR_NUMBER}/`
3. Adds a comment to the PR with the preview URL
4. Automatically cleans up the preview when the PR is closed

This allows reviewers to test changes without needing to check out the PR locally.

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

The presentation uses a custom theme located in `src/presentation/css/custom-theme.css`. You can modify colors, fonts, and styling by editing this file.

## Built with

- [reveal.js](https://revealjs.com/) - HTML presentation framework
- Custom CSS theming with modern design principles
- Google Fonts (Inter & JetBrains Mono)

## License

MIT - See LICENSE file for details.