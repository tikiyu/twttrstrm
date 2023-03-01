import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'shortNumber'
})
export class ShortNumberPipe implements PipeTransform {
  transform(value: number): string {
    const suffixes = ['', 'K', 'M', 'B', 'T'];
    const precision = 2;

    if (value < 1000) {
      return value.toString();
    }

    const exp = Math.floor(Math.log10(value) / 3);
    const roundedValue = (value / Math.pow(1000, exp)).toFixed(precision);

    return roundedValue + suffixes[exp];
  }
}
