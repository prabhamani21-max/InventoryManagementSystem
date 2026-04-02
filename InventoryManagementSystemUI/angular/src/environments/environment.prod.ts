import packageInfo from '../../package.json';

export const environment = {
  appVersion: packageInfo.version,
  production: true,
  apiUrl: 'https://inventorymanagementsystem-bpwm.onrender.com/api',
  signalRHubUrl: 'https://inventorymanagementsystem-bpwm.onrender.com/hubs/user',
  saleOrderHubUrl: 'https://inventorymanagementsystem-bpwm.onrender.com/hubs/saleorder'
};
