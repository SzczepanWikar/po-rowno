export interface PaymentLink {
  href: string;
  rel: string;
  method: string;
}

export type PaymentLinks = PaymentLink[];
