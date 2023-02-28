import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Pipe({
  name: 'atMention'
})
export class AtMentionPipe implements PipeTransform {

  constructor(private sanitizer: DomSanitizer) {}

  transform(value: string): SafeHtml {
    const regex = /@(\w+)/g;
    const replacedValue = value.replace(regex, '<a class="text-info" href="https://twitter.com/$1">@$1</a>');
    console.log(replacedValue);
    return this.sanitizer.bypassSecurityTrustHtml(replacedValue);
  }

}
