using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NqlParser;

namespace YacqlEngine.Queries.Common {

    public class FieldSelectionExtractor {

        public static List<FieldSelection> ExtractFields(NqlParser.FieldSelectionContext context) {

            var fields = new List<FieldSelection>();

            if (context == null)
                return fields;

            foreach (var fieldCtx in context.children.OfType<NqlParser.FieldContext>()) {

                if (fieldCtx is NqlParser.AggregateFieldContext aggregateField) {
                    fields.Add(new FieldSelection {
                        Field = aggregateField.NAME().GetText(),
                        Aggregation = aggregateField.GetChild(0).GetText()
                    });
                }
                else if (fieldCtx is NqlParser.NameFieldContext nameField) {
                    fields.Add(new FieldSelection {
                        Field = nameField.NAME().GetText(),
                        Aggregation = null
                    });
                }
            }

            return fields;
        }
    }
}
