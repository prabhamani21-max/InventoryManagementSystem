export interface NavigationItem {
  id: string;
  title: string;
  type: 'item' | 'collapse' | 'group';
  translate?: string;
  icon?: string;
  hidden?: boolean;
  url?: string;
  classes?: string;
  exactMatch?: boolean;
  external?: boolean;
  target?: boolean;
  breadcrumbs?: boolean;
  badge?: {
    title?: string;
    type?: string;
  };
  children?: NavigationItem[];
}

export const NavigationItems: NavigationItem[] = [
  {
    id: 'navigation',
    title: 'Navigation',
    type: 'group',
    icon: 'icon-group',
    children: [
      {
        id: 'dashboard',
        title: 'Dashboard',
        type: 'item',
        url: 'analytics',
        icon: 'feather icon-home'
      },
      {
        id: 'category',
        title: 'Category',
        type: 'item',
        url: 'category',
        icon: 'feather icon-folder'
      },
      {
        id: 'metal',
        title: 'Metal',
        type: 'item',
        url: 'metal',
        icon: 'feather icon-layers'
      },
      {
        id: 'purity',
        title: 'Purity',
        type: 'item',
        url: 'purity',
        icon: 'feather icon-award'
      },
      {
        id: 'stone',
        title: 'Stones',
        type: 'item',
        url: 'stone',
        icon: 'feather icon-star'
      },
       {
        id: 'supplier',
        title: 'Supplier',
        type: 'item',
        url: 'supplier',
        icon: 'feather icon-users'
      },
       {
        id: 'stoneratehistory',
        title: 'StoneRate',
        type: 'item',
        url: 'stoneratehistory',
        icon: 'feather icon-credit-card'
      },
      {
        id: 'metalratehistory',
        title: 'MetalRate',
        type: 'item',
        url: 'metalratehistory',
        icon: 'feather icon-credit-card'
      },
      {
        id: 'jewellery',
        title: 'JewelleryList',
        type: 'item',
        url: 'jewellery',
        icon: 'feather icon-award'
      },
       {
        id: 'itemstock',
        title: 'Stock',
        type: 'item',
        url: 'itemstock',
        icon: 'feather icon-package'
      },
      {
        id: 'saleorder',
        title: 'Sale Order',
        type: 'item',
        url: 'saleorder',
        icon: 'feather icon-shopping-cart'
      },
      {
        id: 'saleorderitem',
        title: 'Sale Order Item',
        type: 'item',
        url: 'saleorderitem',
        icon: 'feather icon-list'
      },
      {
        id: 'invoice',
        title: 'Invoice',
        type: 'item',
        url: 'invoice',
        icon: 'feather icon-file-text'
      },
      {
        id: 'invoiceitem',
        title: 'Invoice Item',
        type: 'item',
        url: 'invoiceitem',
        icon: 'feather icon-file-plus'
      },
      {
        id: 'payment',
        title: 'Payment',
        type: 'item',
        url: 'payment',
        icon: 'feather icon-dollar-sign'
      },
      {
        id: 'user',
        title: 'User',
        type: 'item',
        url: 'user',
        icon: 'feather icon-user'
      },
      {
        id: 'role',
        title: 'Role',
        type: 'item',
        url: 'role',
        icon: 'feather icon-shield'
      },
      {
        id: 'warehouse',
        title: 'Warehouse',
        type: 'item',
        url: 'warehouse',
        icon: 'feather icon-home'
      },
      {
        id: 'exchange',
        title: 'Exchange',
        type: 'item',
        url: 'exchange',
        icon: 'feather icon-repeat'
      }
    ]
  },
  
];
