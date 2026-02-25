import packageInfo from '../../package.json';

export const environment = {
  appVersion: packageInfo.version,
  production: true,
  apiUrl: 'http://localhost:4200',
  signalRHubUrl: 'http://localhost:4200/hubs/user',
  saleOrderHubUrl: 'http://localhost:4200/hubs/saleorder'
};
