import { Routes } from '@angular/router';
import { adminGuard } from './core/guards/admin.guard';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    data: { titulo: 'Iniciar sesión', sinShell: true },
    loadComponent: () => import('./features/auth/login/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'olvide-contrasena',
    data: { titulo: 'Recuperar contraseña', sinShell: true },
    loadComponent: () =>
      import('./features/auth/olvide-contrasena/olvide-contrasena.component').then(
        (m) => m.OlvideContrasenaComponent
      )
  },
  {
    path: 'restablecer-contrasena',
    data: { titulo: 'Restablecer contraseña', sinShell: true },
    loadComponent: () =>
      import('./features/auth/restablecer-contrasena/restablecer-contrasena.component').then(
        (m) => m.RestablecerContrasenaComponent
      )
  },
  {
    path: '',
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'perfil',
        data: { titulo: 'Mi perfil' },
        loadComponent: () => import('./features/perfil/perfil.component').then((m) => m.PerfilComponent)
      },
      {
        path: 'dashboard',
        data: { titulo: 'Dashboard' },
        loadComponent: () => import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent)
      },
      {
        path: 'transacciones',
        data: { titulo: 'Transacciones' },
        loadComponent: () =>
          import('./features/transacciones/listado/listado-transacciones.component').then(
            (m) => m.ListadoTransaccionesComponent
          )
      },
      {
        path: 'transacciones/nuevo-ingreso',
        data: { titulo: 'Registrar ingreso' },
        loadComponent: () =>
          import('./features/transacciones/registrar-ingreso/registrar-ingreso.component').then(
            (m) => m.RegistrarIngresoComponent
          )
      },
      {
        path: 'fondos',
        data: { titulo: 'Fondos' },
        loadComponent: () => import('./features/fondos/fondos.component').then((m) => m.FondosComponent)
      },
      {
        path: 'cultos',
        data: { titulo: 'Cultos' },
        loadComponent: () => import('./features/cultos/cultos.component').then((m) => m.CultosComponent)
      },
      {
        path: 'miembros',
        data: { titulo: 'Miembros' },
        loadComponent: () => import('./features/miembros/miembros.component').then((m) => m.MiembrosComponent)
      },
      {
        path: 'miembros/:id',
        data: { titulo: 'Aportes del miembro' },
        loadComponent: () =>
          import('./features/miembros/detalle/miembro-detalle.component').then((m) => m.MiembroDetalleComponent)
      },
      {
        path: 'ofrendas-especiales',
        data: { titulo: 'Ofrendas especiales' },
        loadComponent: () =>
          import('./features/ofrendas-especiales/ofrendas-especiales.component').then(
            (m) => m.OfrendasEspecialesComponent
          )
      },
      {
        path: 'ofrendas-especiales/:id',
        data: { titulo: 'Compromisos de la campaña' },
        loadComponent: () =>
          import('./features/ofrendas-especiales/detalle/ofrenda-especial-detalle.component').then(
            (m) => m.OfrendaEspecialDetalleComponent
          )
      },
      {
        path: 'reportes',
        data: { titulo: 'Reportes' },
        loadComponent: () =>
          import('./features/reportes/balance-por-fondo/balance-por-fondo.component').then(
            (m) => m.BalancePorFondoComponent
          )
      },
      {
        path: 'usuarios',
        canActivate: [adminGuard],
        data: { titulo: 'Usuarios' },
        loadComponent: () => import('./features/usuarios/usuarios.component').then((m) => m.UsuariosComponent)
      }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
