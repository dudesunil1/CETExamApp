# Question Bank Module - Complete Guide

## Overview

The enhanced Question Bank module supports text and images for questions, options, and explanations, including mathematical equations using LaTeX.

## Question Types

### 1. MCQ (Multiple Choice Question)
- **Description**: Standard multiple choice with 4 options (A, B, C, D)
- **Options**: 3 wrong options + 1 correct option = 4 total
- **Use Case**: General assessment questions
- **Example**: "What is the capital of India?" with options A-D

### 2. True/False
- **Description**: Binary choice question
- **Options**: True or False
- **Use Case**: Yes/No, fact verification questions
- **Example**: "The earth is flat." (Answer: False)

### 3. MCQ with "All of the above"
- **Description**: MCQ where option D is automatically "All of the above"
- **Options**: 3 options (A, B, C) + D is auto-set to "All of the above"
- **Use Case**: When you want to test if students know multiple facts
- **Example**: "Which are noble gases?" A) Helium, B) Neon, C) Argon, D) All of the above

---

## Question Structure

### Every Question Includes:

1. **Topic** (Required)
   - Links question to specific topic
   - Topics are subject-wise and class-wise

2. **Question Text** (Required)
   - Main question content
   - Supports plain text
   - Supports LaTeX equations
   - Can be supplemented with image

3. **Question Image** (Optional)
   - Upload image for diagrams, graphs
   - Supports complex equations as images
   - Used when text alone isn't sufficient

4. **Options** (For MCQ)
   - **Option A**: Text + Optional Image
   - **Option B**: Text + Optional Image
   - **Option C**: Text + Optional Image
   - **Option D**: Text + Optional Image OR "All of the above"

5. **Correct Answer** (Required)
   - For MCQ: A, B, C, or D
   - For True/False: "True" or "False"

6. **Explanation** (Optional)
   - Text explanation of the answer
   - Helps students understand
   - Can include LaTeX equations

7. **Explanation Image** (Optional)
   - Visual explanation
   - Diagrams showing solution steps

---

## Image Support

### Where Images Can Be Added

1. **Question Image**: Main question diagram/equation
2. **Option A Image**: Visual option A
3. **Option B Image**: Visual option B
4. **Option C Image**: Visual option C
5. **Option D Image**: Visual option D
6. **Explanation Image**: Visual explanation

### Image Upload Specifications

- **File Types**: JPG, JPEG, PNG, GIF
- **Storage Location**: `wwwroot/uploads/questions/`
  - `questions/` - Question images
  - `options/` - Option images
  - `explanations/` - Explanation images
- **File Naming**: `{GUID}_{OriginalFileName}.ext`
- **Display**: Responsive with max-width constraints

### Use Cases for Images

1. **Diagrams**: Circuit diagrams, geometric figures
2. **Graphs**: Mathematical graphs, charts
3. **Complex Equations**: When LaTeX is too complex
4. **Visual Options**: Images as answer choices
5. **Step-by-step Solutions**: Visual explanations

---

## Math Equations Support

### LaTeX Integration

The application includes **MathJax** for rendering mathematical equations.

### How to Write Math Equations

#### Inline Equations (within text)
```
Use $...$ or \(...\) for inline
Example: The formula $E = mc^2$ is famous.
```

#### Display Equations (on separate line)
```
Use $$...$$ or \[...\] for display
Example: $$\int_{0}^{\infty} e^{-x} dx = 1$$
```

### Example LaTeX Equations

**Quadratic Formula:**
```
$$x = \frac{-b \pm \sqrt{b^2 - 4ac}}{2a}$$
```

**Physics Equation:**
```
$$F = ma$$
```

**Complex Fraction:**
```
$$\frac{x^2 + y^2}{z^2 - w^2}$$
```

**Greek Letters:**
```
$$\alpha, \beta, \gamma, \theta, \pi$$
```

**Integral:**
```
$$\int_{a}^{b} f(x) dx$$
```

**Summation:**
```
$$\sum_{i=1}^{n} i = \frac{n(n+1)}{2}$$
```

---

## Creating Questions

### MCQ Question Example

**Question Text:**
```
Calculate the value of $$\int_{0}^{1} x^2 dx$$
```

**Question Image**: (Optional - could upload graph image)

**Options:**
- **A**: $\frac{1}{3}$ (Correct)
- **B**: $\frac{1}{2}$
- **C**: $1$
- **D**: $2$

**Correct Answer**: A

**Explanation:**
```
Using integration: $$\int x^2 dx = \frac{x^3}{3} + C$$
Evaluating from 0 to 1: $$[\frac{x^3}{3}]_0^1 = \frac{1}{3} - 0 = \frac{1}{3}$$
```

---

### MCQ with "All of the above" Example

**Question Text:**
```
Which of the following are prime numbers?
```

**Options:**
- **A**: 2
- **B**: 3
- **C**: 5
- **D**: All of the above (Auto-set)

**Correct Answer**: D

**Explanation**:
```
2, 3, and 5 are all prime numbers, so the answer is "All of the above".
```

---

### True/False Question Example

**Question Text:**
```
The speed of light in vacuum is approximately $3 \times 10^8$ m/s.
```

**Correct Answer**: True

**Explanation**:
```
The speed of light in vacuum is exactly 299,792,458 m/s, 
which is approximately $3 \times 10^8$ m/s.
```

---

## Database Schema

### Question Model (Enhanced)

```csharp
public class Question
{
    public int Id { get; set; }
    
    // Question content
    public string QuestionText { get; set; }              // Text + LaTeX
    public string? QuestionImagePath { get; set; }        // NEW: Image path
    
    // Question metadata
    public QuestionType QuestionType { get; set; }        // MCQ/TrueFalse/MCQWithAllOfAbove
    public DifficultyLevel DifficultyLevel { get; set; }
    public int TopicId { get; set; }
    
    // Options with images
    public string? OptionA { get; set; }
    public string? OptionAImagePath { get; set; }         // NEW
    public string? OptionB { get; set; }
    public string? OptionBImagePath { get; set; }         // NEW
    public string? OptionC { get; set; }
    public string? OptionCImagePath { get; set; }         // NEW
    public string? OptionD { get; set; }
    public string? OptionDImagePath { get; set; }         // NEW
    
    // Answer and explanation
    public string CorrectAnswer { get; set; }
    public string? Explanation { get; set; }              // Text + LaTeX
    public string? ExplanationImagePath { get; set; }     // NEW: Image path
    
    // Other fields
    public int Marks { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
}
```

### New Columns Added

| Column | Type | Purpose |
|--------|------|---------|
| QuestionImagePath | nvarchar(500) | Path to question image |
| OptionAImagePath | nvarchar(500) | Path to option A image |
| OptionBImagePath | nvarchar(500) | Path to option B image |
| OptionCImagePath | nvarchar(500) | Path to option C image |
| OptionDImagePath | nvarchar(500) | Path to option D image |
| ExplanationImagePath | nvarchar(500) | Path to explanation image |

---

## Question Types Enum (Updated)

```csharp
public enum QuestionType
{
    MCQ = 0,                    // Standard Multiple Choice
    TrueFalse = 1,              // True/False Question
    MCQWithAllOfAbove = 2       // MCQ with "All of the above" as D
}
```

---

## Form Features

### Create Question Form

**Sections:**
1. **Question Settings**
   - Topic selection (dropdown)
   - Question type (MCQ/True-False/MCQ with All)
   - Difficulty level (Easy/Medium/Hard)

2. **Question Content**
   - Question text (textarea with LaTeX support)
   - Question image upload (optional)

3. **Answer Options** (for MCQ)
   - 4 option cards (A, B, C, D)
   - Each option has:
     - Text area for option text
     - Image upload for option image
   - Option D special handling for "All of above" type

4. **True/False** (for True/False questions)
   - Instructions to enter "True" or "False"

5. **Correct Answer & Explanation**
   - Correct answer field
   - Explanation text (with LaTeX)
   - Explanation image upload

6. **Additional Settings**
   - Marks (1-100)
   - Active status

### Dynamic Form Behavior

**When MCQ selected:**
- Shows all 4 options (A, B, C, D)
- All options editable
- Hint: "Enter A, B, C, or D"

**When True/False selected:**
- Hides option cards
- Shows True/False instructions
- Hint: "Enter True or False"

**When MCQ (All of Above) selected:**
- Shows options A, B, C
- Hides Option D input (auto-set to "All of the above")
- Shows badge: "Auto: All of the above"
- Hint: "Enter A, B, C, or D"

---

## Sample Questions

### Sample 1: MCQ with LaTeX
```
Topic: Calculus (Mathematics - 12th)
Type: MCQ
Difficulty: Medium

Question: 
Find the derivative of $$f(x) = x^3 + 2x^2 - 5x + 7$$

Options:
A) $$3x^2 + 4x - 5$$ (Correct)
B) $$3x^2 + 2x - 5$$
C) $$x^2 + 4x - 5$$
D) $$3x^2 + 4x + 5$$

Correct Answer: A

Explanation:
Using power rule: $$\frac{d}{dx}(x^n) = nx^{n-1}$$
$$f'(x) = 3x^2 + 4x - 5$$

Marks: 2
```

### Sample 2: MCQ with Image
```
Topic: Electric Charges (Physics - 12th)
Type: MCQ

Question: 
Identify the correct circuit diagram for a series connection:
[Upload: circuit_series.png]

Options:
A) [Upload: circuit_a.png]
B) [Upload: circuit_b.png] (Correct)
C) [Upload: circuit_c.png]
D) [Upload: circuit_d.png]

Correct Answer: B

Explanation:
In a series circuit, components are connected end-to-end.
[Upload: series_explanation.png]

Marks: 1
```

### Sample 3: MCQ with All of Above
```
Topic: Biological Classification (Biology - 11th)
Type: MCQ with All of Above
Difficulty: Easy

Question: 
Which of the following are characteristics of living organisms?

Options:
A) Growth
B) Reproduction
C) Metabolism
D) All of the above (Auto-set)

Correct Answer: D

Explanation:
All three - growth, reproduction, and metabolism - are 
fundamental characteristics of living organisms.

Marks: 1
```

### Sample 4: True/False
```
Topic: Chemical Bonding (Chemistry - 11th)
Type: True/False
Difficulty: Easy

Question:
Ionic bonds are formed by sharing of electrons.

Correct Answer: False

Explanation:
Ionic bonds are formed by transfer of electrons, not sharing.
Covalent bonds are formed by sharing of electrons.

Marks: 1
```

---

## Image Upload Workflow

### Creating Question with Images

1. **Fill Question Text**
   - Enter question with LaTeX equations if needed
   
2. **Upload Question Image** (if needed)
   - Click "Question Image" file input
   - Select image file (diagram, graph, etc.)
   
3. **Fill Options**
   - Enter text for each option
   - Optionally upload images for options
   
4. **Upload Explanation Image** (if needed)
   - Select image showing solution steps
   
5. **Submit**
   - All images uploaded automatically
   - Unique filenames generated
   - Paths stored in database

### Editing Question with Images

1. **Current Images Displayed**
   - Question image shown (if exists)
   - Option images shown (if exist)
   - Explanation image shown (if exists)
   
2. **Update Images** (optional)
   - Upload new image to replace
   - Leave empty to keep current
   - Old images deleted automatically

3. **Submit**
   - Only new images uploaded
   - Old images preserved if not replaced

---

## Storage Structure

```
wwwroot/uploads/questions/
├── questions/          # Question images
│   ├── .gitkeep
│   ├── abc123_diagram1.png
│   └── def456_graph1.jpg
├── options/           # Option images
│   ├── .gitkeep
│   ├── ghi789_option_a.png
│   └── jkl012_option_b.jpg
└── explanations/      # Explanation images
    ├── .gitkeep
    ├── mno345_solution1.png
    └── pqr678_steps.jpg
```

---

## Math Equations Guide

### Basic LaTeX Syntax

#### Powers and Subscripts
```latex
$$x^2$$           → x²
$$x_1$$           → x₁
$$x^{2y}$$        → x^(2y)
$$x_{i,j}$$       → x_(i,j)
```

#### Fractions
```latex
$$\frac{a}{b}$$                    → a/b
$$\frac{x^2 + 1}{x - 1}$$          → (x²+1)/(x-1)
```

#### Square Roots
```latex
$$\sqrt{x}$$                       → √x
$$\sqrt[3]{x}$$                    → ³√x
$$\sqrt{x^2 + y^2}$$              → √(x²+y²)
```

#### Greek Letters
```latex
$$\alpha, \beta, \gamma$$         → α, β, γ
$$\theta, \pi, \omega$$           → θ, π, ω
$$\Delta, \Sigma, \Omega$$        → Δ, Σ, Ω
```

#### Trigonometric Functions
```latex
$$\sin(x), \cos(x), \tan(x)$$
$$\sin^2(x) + \cos^2(x) = 1$$
```

#### Calculus
```latex
$$\int_{a}^{b} f(x) dx$$          → Integral
$$\frac{dy}{dx}$$                 → Derivative
$$\lim_{x \to \infty} f(x)$$      → Limit
```

#### Summation and Products
```latex
$$\sum_{i=1}^{n} i$$              → Summation
$$\prod_{i=1}^{n} i$$             → Product
```

#### Matrices
```latex
$$\begin{bmatrix} a & b \\ c & d \end{bmatrix}$$
```

---

## Example Questions with LaTeX

### Physics - Kinematics
```
Question: A body starts from rest and accelerates uniformly at 
$a = 2 \text{ m/s}^2$. What is the distance traveled in 5 seconds?

Use formula: $$s = ut + \frac{1}{2}at^2$$

Options:
A) 10 m
B) 20 m
C) 25 m (Correct)
D) 50 m

Explanation:
Given: $u = 0$, $a = 2$, $t = 5$
$$s = 0 \times 5 + \frac{1}{2} \times 2 \times 5^2$$
$$s = 0 + 1 \times 25 = 25 \text{ m}$$
```

### Mathematics - Algebra
```
Question: Solve for x: $$2x + 5 = 15$$

Options:
A) x = 5 (Correct)
B) x = 10
C) x = 7.5
D) x = 2.5

Explanation:
$$2x + 5 = 15$$
$$2x = 15 - 5$$
$$2x = 10$$
$$x = 5$$
```

### Chemistry - Mole Concept
```
Question: How many moles are in 18g of water ($H_2O$)?
(Molecular weight of $H_2O$ = 18 g/mol)

Options:
A) 1 mole (Correct)
B) 2 moles
C) 18 moles
D) 0.5 moles

Explanation:
$$\text{Number of moles} = \frac{\text{Mass}}{\text{Molecular Weight}}$$
$$= \frac{18}{18} = 1 \text{ mole}$$
```

---

## Image and Text Combinations

### Example: Geometry Question

**Question Text:**
```
In the triangle shown below, what is the value of angle θ?
```

**Question Image:**
- Upload: triangle_diagram.png (showing triangle with marked angle)

**Options:**
- A) 30°
- B) 45° (Correct)
- C) 60°
- D) 90°

**Explanation Text:**
```
Using angle sum property of triangle: $$\alpha + \beta + \theta = 180°$$
```

**Explanation Image:**
- Upload: triangle_solution.png (showing calculation steps)

---

## Question Bank Features

### List View
- ✅ Shows question preview with thumbnail
- ✅ Filters by topic and difficulty
- ✅ Displays question type badge
- ✅ View/Edit/Delete actions

### Create View
- ✅ Dynamic form based on question type
- ✅ Image upload for all components
- ✅ LaTeX equation support
- ✅ Validation and hints

### Edit View
- ✅ Shows current images
- ✅ Optional image updates
- ✅ Preserves existing images
- ✅ Delete old images on update

### Details View
- ✅ Full question display
- ✅ All images rendered
- ✅ LaTeX equations rendered
- ✅ Correct answer highlighted
- ✅ Explanation shown

### Delete View
- ✅ Shows question preview
- ✅ Deletes all associated images
- ✅ Confirmation required

---

## Best Practices

### Writing Questions

1. ✅ Use clear, concise language
2. ✅ Add LaTeX for mathematical expressions
3. ✅ Upload images for complex diagrams
4. ✅ Provide detailed explanations
5. ✅ Set appropriate difficulty levels

### Using Images

1. ✅ Use images for visual concepts
2. ✅ Keep file sizes reasonable (<500KB)
3. ✅ Use clear, high-resolution images
4. ✅ Label diagrams clearly
5. ✅ Use images when LaTeX is insufficient

### LaTeX Equations

1. ✅ Test equations before submitting
2. ✅ Use proper LaTeX syntax
3. ✅ Use $...$ for inline, $$...$$ for display
4. ✅ Keep equations readable
5. ✅ Add text explanations alongside

### Option Organization

1. ✅ Mix up correct answer position (not always A)
2. ✅ Make distractors plausible
3. ✅ Keep options similar in length
4. ✅ Use consistent formatting
5. ✅ For "All of above", ensure all other options are correct

---

## Migration Required

After updating the Question model, create migration:

```bash
# Create migration
dotnet ef migrations add EnhanceQuestionBankWithImages

# Update database
dotnet ef database update
```

This adds:
- QuestionImagePath column
- OptionAImagePath column
- OptionBImagePath column
- OptionCImagePath column
- OptionDImagePath column
- ExplanationImagePath column

---

## Troubleshooting

### Images Not Displaying
**Issue**: Uploaded images don't show
**Solutions**:
- Check upload directory exists
- Verify file permissions
- Confirm path in database
- Check image file is valid

### LaTeX Not Rendering
**Issue**: Math equations show as plain text
**Solutions**:
- Ensure MathJax is loaded (check browser console)
- Use correct LaTeX syntax ($$...$$ for display)
- Wait for page to fully load
- Check for JavaScript errors

### File Upload Fails
**Issue**: Cannot upload image
**Solutions**:
- Check file size (<10MB recommended)
- Verify file is valid image
- Ensure sufficient disk space
- Check write permissions on uploads folder

---

## Tips & Tricks

### For Physics Questions
- Use images for circuit diagrams, free body diagrams
- Use LaTeX for equations and formulas
- Include unit conversions in explanations

### For Chemistry Questions
- Upload structural formulas as images
- Use LaTeX for chemical equations
- Include molecular diagrams

### For Mathematics Questions
- Use LaTeX extensively for equations
- Upload graphs as images
- Show step-by-step solutions in explanations

### For Biology Questions
- Upload diagrams (cell structure, organs, etc.)
- Use images for classification charts
- Include labeled diagrams in explanations

---

## Summary

### Enhanced Features

✅ **3 Question Types**: MCQ, True/False, MCQ with All of Above  
✅ **Image Support**: Questions, Options, Explanations  
✅ **LaTeX Equations**: Full MathJax integration  
✅ **Mixed Content**: Text + Images + Equations  
✅ **File Management**: Auto upload, cleanup, unique naming  
✅ **Responsive Display**: Images scale properly  
✅ **Dynamic Forms**: UI adapts to question type  

### What You Can Create

- ✅ Text-only questions
- ✅ Image-only questions
- ✅ Mixed text and image questions
- ✅ Questions with math equations (LaTeX)
- ✅ Questions with diagrams
- ✅ Questions with graphs and charts
- ✅ Comprehensive explanations with visuals

---

**Version**: 2.3.0  
**Last Updated**: October 2025  
**Feature**: Enhanced Question Bank with Images and LaTeX

