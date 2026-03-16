import { RoleEnum } from "src/app/core/enums/role.enum";

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
  roleId?: number[]; 
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
        icon: 'feather icon-home',
        roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

      },
      {
        id: 'category',
        title: 'Category',
        type: 'item',
        url: 'category',
        icon: 'feather icon-folder',
        roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

      },
       {
         id: 'metal',
         title: 'Metal',
         type: 'item',
         url: 'metal',
         icon: 'feather icon-layers',
         roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

       },
       {
         id: 'generalstatus',
         title: 'General Status',
         type: 'item',
         url: 'generalstatus',
         icon: 'feather icon-alert-circle',
                 roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

       },
       {
         id: 'purity',
         title: 'Purity',
         type: 'item',
         url: 'purity',
         icon: 'feather icon-award',
                 roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

       },
      {
        id: 'stone',
        title: 'Stones',
        type: 'item',
        url: 'stone',
        icon: 'feather icon-star',
                roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

      },
       {
        id: 'supplier',
        title: 'Supplier',
        type: 'item',
        url: 'supplier',
        icon: 'feather icon-users',
                roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

      },
       {
        id: 'stoneratehistory',
        title: 'StoneRate',
        type: 'item',
        url: 'stoneratehistory',
        icon: 'feather icon-credit-card',
                roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

      },
      {
        id: 'metalratehistory',
        title: 'MetalRate',
        type: 'item',
        url: 'metalratehistory',
        icon: 'feather icon-credit-card',
                roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

      },
      {
        id: 'jewellery',
        title: 'JewelleryList',
        type: 'item',
        url: 'jewellery',
        icon: 'feather icon-award',
                roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

      },
       {
        id: 'itemstock',
        title: 'Stock',
        type: 'item',
        url: 'itemstock',
        icon: 'feather icon-package',
                roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

      },
      {
        id: 'saleorder',
        title: 'Sale Order',
        type: 'item',
        url: 'saleorder',
        icon: 'feather icon-shopping-cart',
                roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

      },
      {
        id: 'saleorderitem',
        title: 'Sale Order Item',
        type: 'item',
        url: 'saleorderitem',
        icon: 'feather icon-list',
                roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

      },
      {
        id: 'invoice',
        title: 'Invoice',
        type: 'item',
        url: 'invoice',
        icon: 'feather icon-file-text',
                roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

      },
      {
        id: 'invoiceitem',
        title: 'Invoice Item',
        type: 'item',
        url: 'invoiceitem',
        icon: 'feather icon-file-plus',
                roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

      },
      {
        id: 'payment',
        title: 'Payment',
        type: 'item',
        url: 'payment',
        icon: 'feather icon-dollar-sign',
                roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

      },
      {
        id: 'user',
        title: 'User',
        type: 'item',
        url: 'user',
        icon: 'feather icon-user',
                roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager,RoleEnum.Sales], // SuperAdmin (1), Admin (2)

      },
    
      {
        id: 'user',
        title: 'Userkyc',
        type: 'item',
        url: 'userkyc',
        icon: 'feather icon-users',
                roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager, RoleEnum.Sales], // SuperAdmin (1), Admin (2)

      },

      {
        id: 'role',
        title: 'Role',
        type: 'item',
        url: 'role',
        icon: 'feather icon-shield',
                roleId: [RoleEnum.SuperAdmin], // SuperAdmin (1), Admin (2)

      },
      {
        id: 'warehouse',
        title: 'Warehouse',
        type: 'item',
        url: 'warehouse',
        icon: 'feather icon-home',
                roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager], // SuperAdmin (1), Admin (2)

      },
      {
        id: 'exchange',
        title: 'Exchange',
        type: 'item',
        url: 'exchange',
        icon: 'feather icon-repeat',
                roleId: [RoleEnum.SuperAdmin,RoleEnum.Manager,RoleEnum.Sales], // SuperAdmin (1), Admin (2)

      }
    ]
  },
  
];
