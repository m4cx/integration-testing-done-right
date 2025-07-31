#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

// Helper function to create directory if it doesn't exist
function ensureDir(dir) {
    if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir, { recursive: true });
    }
}

// Helper function to copy file
function copyFile(src, dest) {
    ensureDir(path.dirname(dest));
    fs.copyFileSync(src, dest);
    console.log(`Copied: ${src} -> ${dest}`);
}

// Helper function to copy directory recursively
function copyDir(src, dest) {
    ensureDir(dest);
    const entries = fs.readdirSync(src, { withFileTypes: true });
    
    for (const entry of entries) {
        const srcPath = path.join(src, entry.name);
        const destPath = path.join(dest, entry.name);
        
        if (entry.isDirectory()) {
            copyDir(srcPath, destPath);
        } else {
            copyFile(srcPath, destPath);
        }
    }
}

console.log('Building presentation for deployment...');

// Clean and create dist directory
const distDir = './dist';
if (fs.existsSync(distDir)) {
    fs.rmSync(distDir, { recursive: true });
}
ensureDir(distDir);

// Copy main files
copyFile('./src/presentation/index.html', './dist/index.html');
copyDir('./src/presentation/css', './dist/css');
copyDir('./src/presentation/diagrams', './dist/diagrams');

// Copy reveal.js files
const revealSrc = './node_modules/reveal.js';
const revealDest = './dist/lib/reveal.js';

// Copy reveal.js dist files
copyDir(`${revealSrc}/dist`, `${revealDest}/dist`);

// Copy specific plugins we use
const plugins = ['notes', 'markdown', 'highlight'];
for (const plugin of plugins) {
    copyDir(`${revealSrc}/plugin/${plugin}`, `${revealDest}/plugin/${plugin}`);
}

// Update index.html to use new paths
let indexHtml = fs.readFileSync('./dist/index.html', 'utf8');

// Replace node_modules references with lib references
indexHtml = indexHtml.replace(/node_modules\/reveal\.js/g, 'lib/reveal.js');

fs.writeFileSync('./dist/index.html', indexHtml);

console.log('Build completed! Files are ready in ./dist/');
console.log('You can test the build with: npm run build:serve');