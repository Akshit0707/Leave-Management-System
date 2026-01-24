function getApiUrl() {
  // Netlify and browser: use window.API_URL if set
  if (typeof window !== 'undefined' && window['API_URL']) {
    return window['API_URL'];
  }
  // Node.js/SSR: use process.env.API_URL if set
  if (typeof process !== 'undefined' && process.env && process.env['API_URL']) {
    return process.env['API_URL'];
  }
  // Browser: check location
  if (typeof window !== 'undefined' && window.location) {
    return window.location.hostname === 'localhost'
      ? 'http://localhost:8080'
      : 'https://leave-management-system.up.railway.app';
  }
  // Fallback
  return 'https://leave-management-system.up.railway.app';
}

export const environment = {
  production: false,
  apiUrl: getApiUrl()
};

