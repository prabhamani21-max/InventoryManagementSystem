// angular import
import { ChangeDetectorRef, Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterModule } from '@angular/router';

// project import
import { FormsModule, ReactiveFormsModule, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { RoleEnum } from 'src/app/core/enums/role.enum';

@Component({
  selector: 'app-sign-in',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.scss']
})
export class SignInComponent {
  private cd = inject(ChangeDetectorRef);
  signInForm!: UntypedFormGroup;
  submitted: boolean = false;
  isLoading: boolean = false;
  errorMessage: string | null = null;
  showPassword = signal(false);
  private fb = inject(UntypedFormBuilder);
  private authService = inject(AuthenticationService);
  private router = inject(Router);
  private toastr = inject(ToastrService); // Inject ToastrService

  ngOnInit(): void {
    this.signInForm = this.fb.group({
      identifier: ['', [Validators.required]], // Can be email or phone number
      password: ['', [Validators.required]],
    });
  }
  get formValues() {
    return this.signInForm.controls;
  }
  togglePasswordVisibility() {
    this.showPassword.set(!this.showPassword());
  }
  login() {
    this.submitted = true;
    this.errorMessage = null;

    if (this.signInForm.valid) {
      this.isLoading = true;
      const { identifier, password } = this.signInForm.value;

      this.authService.login(identifier, password).subscribe({
        next: () => {
          // Verify token is stored and decoded properly
          const token = this.authService.token;
          const decodedToken = this.authService.getDecodedToken();

          if (token && decodedToken) {
            this.toastr.success('Login successful!');
            const roleId = decodedToken.roleId;
            switch (roleId) {
              case RoleEnum.SuperAdmin.toString():
                this.router.navigate(['jewelleryManagement/admin/analytics']);
                break;
            }
          } else {
            this.toastr.error('Authentication failed. Please try again.');
            this.isLoading = false;
          }
        },
        error: (error) => {
          this.isLoading = false;

          if (error.status === 404) {
            this.toastr.error('Invalid email/phone number or password.');
          } else {
            this.toastr.error('Login failed. Please try again.');
          }
        },       
      });  
    }
  }
}
