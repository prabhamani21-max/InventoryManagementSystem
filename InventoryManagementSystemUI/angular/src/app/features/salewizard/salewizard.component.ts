import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { SaleWizardService, SaleWizardState } from 'src/app/core/services/sale-wizard.service';

/**
 * Step configuration interface
 */
interface StepConfig {
  number: number;
  title: string;
  icon: string;
  path: string;
}

/**
 * SaleWizardComponent
 * Main container component for the multi-step sale wizard
 */
@Component({
  selector: 'app-sale-wizard',
  imports: [CommonModule, SharedModule, RouterModule],
  templateUrl: './salewizard.component.html',
  styleUrl: './salewizard.component.scss',
})
export class SaleWizardComponent implements OnInit, OnDestroy {
  wizardService = inject(SaleWizardService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  
  private destroy$ = new Subject<void>();
  
  // State
  wizardState: SaleWizardState = this.wizardService.getCurrentState();
  
  // Step configuration
  steps: StepConfig[] = [
    { number: 1, title: 'Customer', icon: 'person', path: 'customer' },
    { number: 2, title: 'Sale Order', icon: 'receipt_long', path: 'order' },
    { number: 3, title: 'Items', icon: 'shopping_bag', path: 'items' },
    { number: 4, title: 'Payment', icon: 'payment', path: 'payment' },
    { number: 5, title: 'Invoice', icon: 'description', path: 'invoice' },
  ];

  ngOnInit(): void {
    // Subscribe to wizard state changes
    this.wizardService.state$
      .pipe(takeUntil(this.destroy$))
      .subscribe((state) => {
        this.wizardState = state;
      });
    
    // Sync current step with route
    this.syncStepWithRoute();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Sync the wizard step with the current route
   */
  private syncStepWithRoute(): void {
    this.route.firstChild?.data.subscribe((data) => {
      if (data['step']) {
        const step = data['step'] as number;
        if (this.wizardState.currentStep !== step) {
          // Navigate to correct step based on state
          this.navigateToCurrentStep();
        }
      }
    });
  }

  /**
   * Navigate to the current step's route
   */
  private navigateToCurrentStep(): void {
    const currentStep = this.wizardState.currentStep;
    const stepConfig = this.steps.find(s => s.number === currentStep);
    if (stepConfig) {
      this.router.navigate([stepConfig.path], { relativeTo: this.route });
    }
  }

  /**
   * Check if a step is active
   */
  isStepActive(stepNumber: number): boolean {
    return this.wizardState.currentStep === stepNumber;
  }

  /**
   * Check if a step is completed
   */
  isStepCompleted(stepNumber: number): boolean {
    return this.wizardState.completedSteps.includes(stepNumber) ||
           this.wizardService.isStepComplete(stepNumber);
  }

  /**
   * Check if a step can be navigated to
   */
  canNavigateToStep(stepNumber: number): boolean {
    // Can always go to completed steps or current step
    if (this.isStepCompleted(stepNumber) || this.isStepActive(stepNumber)) {
      return true;
    }
    
    // Can go to next step if current step is valid
    if (stepNumber === this.wizardState.currentStep + 1) {
      return this.wizardService.canProceed();
    }
    
    return false;
  }

  /**
   * Navigate to a specific step
   */
  goToStep(step: StepConfig): void {
    if (this.canNavigateToStep(step.number)) {
      this.wizardService.goToStep(step.number);
      this.router.navigate([step.path], { relativeTo: this.route });
    }
  }

  /**
   * Go to next step
   */
  onNext(): void {
    if (this.wizardService.canProceed()) {
      this.wizardService.nextStep();
      const nextStep = this.steps.find(s => s.number === this.wizardState.currentStep);
      if (nextStep) {
        this.router.navigate([nextStep.path], { relativeTo: this.route });
      }
    }
  }

  /**
   * Go to previous step
   */
  onPrevious(): void {
    this.wizardService.previousStep();
    const prevStep = this.steps.find(s => s.number === this.wizardState.currentStep);
    if (prevStep) {
      this.router.navigate([prevStep.path], { relativeTo: this.route });
    }
  }

  /**
   * Cancel wizard and return to dashboard
   */
  onCancel(): void {
    this.wizardService.resetWizard();
    this.router.navigate(['jewelleryManagement/admin/analytics']);
  }

  /**
   * Complete wizard and return to dashboard
   */
  onComplete(): void {
    this.wizardService.resetWizard();
    this.router.navigate(['jewelleryManagement/admin/analytics']);
  }

  /**
   * Get step CSS class
   */
  getStepClass(stepNumber: number): string {
    if (this.isStepActive(stepNumber)) {
      return 'step-active';
    }
    if (this.isStepCompleted(stepNumber)) {
      return 'step-completed';
    }
    return 'step-pending';
  }

  /**
   * Get progress percentage
   */
  getProgressPercentage(): number {
    const completedCount = this.wizardState.completedSteps.length;
    const currentStep = this.wizardState.currentStep;
    
    // Include current step in progress if it's completed
    const effectiveCompleted = this.wizardService.isStepComplete(currentStep) 
      ? completedCount + 1 
      : completedCount;
    
    return (effectiveCompleted / this.steps.length) * 100;
  }
}
